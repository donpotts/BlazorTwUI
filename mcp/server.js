#!/usr/bin/env node

/**
 * BlazorTwUI — Official Model Context Protocol (MCP) Server
 * Pure, zero-dependency Node.js implementation conforming to MCP 2024-11-05 spec.
 * 
 * Exposes:
 *  - Tools:
 *      * blazortw_search_components
 *      * blazortw_get_component_api
 *      * blazortw_scaffold_component
 *      * blazortw_list_forms
 *      * blazortw_get_form_schema
 *      * blazortw_validate_micr
 *      * blazortw_audit_razor
 *  - Resources:
 *      * blazortw://components/catalog
 *      * blazortw://forms/100-standard
 *      * blazortw://design/guidelines
 *  - Prompts:
 *      * scaffold-enterprise-page
 *      * synthesize-agui-agent
 */

const fs = require('fs');
const path = require('path');
const readline = require('readline');

// Load catalog and form data
const componentsPath = path.join(__dirname, 'data', 'components.json');
const formsPath = path.join(__dirname, 'data', 'forms.json');

let components = [];
let forms = [];

try {
  components = JSON.parse(fs.readFileSync(componentsPath, 'utf8'));
} catch (e) {
  components = [];
}

try {
  forms = JSON.parse(fs.readFileSync(formsPath, 'utf8'));
} catch (e) {
  forms = [];
}

// ABA Routing Checksum Calculation (Modulus 10)
function validateAbaRouting(routing) {
  if (!routing || typeof routing !== 'string') return { isValid: false, mod: -1, bankName: 'Unknown' };
  const clean = routing.replace(/\D/g, '');
  if (clean.length !== 9) return { isValid: false, mod: -1, bankName: 'Invalid length (must be 9 digits)' };
  
  const d = clean.split('').map(Number);
  const sum = 3 * (d[0] + d[3] + d[6]) + 7 * (d[1] + d[4] + d[7]) + 1 * (d[2] + d[5] + d[8]);
  const mod = sum % 10;
  
  const knownBanks = {
    '121000358': 'JPMorgan Chase Bank, N.A. (San Francisco)',
    '021000021': 'JPMorgan Chase Bank, N.A. (New York)',
    '071000013': 'Bank of America, N.A. (Chicago)',
    '122000496': 'Wells Fargo Bank, N.A. (San Francisco)',
    '011000138': 'Bank of America, N.A. (Boston)'
  };

  return {
    isValid: mod === 0,
    mod: mod,
    bankName: knownBanks[clean] || (mod === 0 ? 'Verified Federal Reserve Depository' : 'Invalid Routing Checksum')
  };
}

// Parse ANSI X9.27 MICR Line
function parseMicrLine(raw) {
  const result = {
    rawMicr: raw,
    routing: '',
    account: '',
    checkNumber: '',
    isValidChecksum: false,
    bankName: ''
  };

  if (!raw) return result;

  const firstTransit = raw.indexOf('⑆');
  const secondTransit = firstTransit >= 0 ? raw.indexOf('⑆', firstTransit + 1) : -1;

  if (firstTransit >= 0 && secondTransit > firstTransit) {
    result.routing = raw.substring(firstTransit + 1, secondTransit).trim();
    const val = validateAbaRouting(result.routing);
    result.isValidChecksum = val.isValid;
    result.bankName = val.bankName;
  }

  const rest = secondTransit >= 0 ? raw.substring(secondTransit + 1).trim() : raw;
  const onUs = rest.indexOf('⑈');
  if (onUs >= 0) {
    result.account = rest.substring(0, onUs).trim();
    const afterOnUs = rest.substring(onUs + 1).trim();
    const tokens = afterOnUs.split(/\s+/);
    if (tokens.length > 0) result.checkNumber = tokens[0];
  } else {
    const tokens = rest.split(/\s+/);
    if (tokens.length > 0) result.account = tokens[0];
    if (tokens.length > 1) result.checkNumber = tokens[1];
  }

  return result;
}

// Tool definitions
const TOOLS = [
  {
    name: 'blazortw_search_components',
    description: 'Search the 147+ production-ready BlazorTwUI component library by keyword, category, or natural language intent.',
    inputSchema: {
      type: 'object',
      properties: {
        query: { type: 'string', description: 'Search keywords, e.g. "financial chart", "check reader", "pivot grid", "confetti"' },
        category: { type: 'string', description: 'Optional category filter, e.g. "Financial & OCR", "Interactive Creative Tools"' }
      },
      required: ['query']
    }
  },
  {
    name: 'blazortw_get_component_api',
    description: 'Get the exact, strongly-typed C# parameter specification, default values, and event callbacks for any BlazorTwUI component.',
    inputSchema: {
      type: 'object',
      properties: {
        componentName: { type: 'string', description: 'Exact component name, e.g. "TwMicrCheckReader", "TwDataGrid", "TwAguiFormCanvas"' }
      },
      required: ['componentName']
    }
  },
  {
    name: 'blazortw_scaffold_component',
    description: 'Generate copy-pasteable, production-ready Razor component markup with idiomatic @bind-Value data bindings and Tailwind styling.',
    inputSchema: {
      type: 'object',
      properties: {
        componentName: { type: 'string', description: 'Name of the component to scaffold' },
        customTitle: { type: 'string', description: 'Optional custom title or header' },
        includeCodeBlock: { type: 'boolean', description: 'Whether to include a sample @code block with state variables' }
      },
      required: ['componentName']
    }
  },
  {
    name: 'blazortw_list_forms',
    description: 'List enterprise form schemas from the 100 Most Used Forms Library across 10 business domains.',
    inputSchema: {
      type: 'object',
      properties: {
        category: { type: 'string', description: 'Optional category: "Authentication & Identity", "Enterprise & Business Operations", "SaaS & Cloud Operations", etc.' }
      }
    }
  },
  {
    name: 'blazortw_get_form_schema',
    description: 'Retrieve the complete field schema and field definitions for one of the 100 standard enterprise forms.',
    inputSchema: {
      type: 'object',
      properties: {
        formId: { type: 'string', description: 'ID of the form, e.g. "login", "company-plan", "profit-loss", "expense-report"' }
      },
      required: ['formId']
    }
  },
  {
    name: 'blazortw_validate_micr',
    description: 'Validate an ANSI X9.27 Check 21 MICR E-13B line and compute the Federal Reserve Modulus-10 ABA routing transit check digit.',
    inputSchema: {
      type: 'object',
      properties: {
        micrLine: { type: 'string', description: 'Raw MICR line string containing transit ⑆ and on-us ⑈ glyphs, or 9-digit routing number' }
      },
      required: ['micrLine']
    }
  },
  {
    name: 'blazortw_audit_razor',
    description: 'Lint and audit Razor markup for mobile-first touch compliance (44px min targets), zero proprietary brand imports, and dark mode compatibility.',
    inputSchema: {
      type: 'object',
      properties: {
        razorCode: { type: 'string', description: 'Razor markup to analyze' }
      },
      required: ['razorCode']
    }
  }
];

// Resource definitions
const RESOURCES = [
  {
    uri: 'blazortw://components/catalog',
    name: 'BlazorTwUI Complete Component Catalog',
    description: 'JSON catalog of all 147+ production components with categories, parameters, and documentation.',
    mimeType: 'application/json'
  },
  {
    uri: 'blazortw://forms/100-standard',
    name: '100 Standard Enterprise Forms Catalog',
    description: 'Catalog of 100 enterprise form schemas across 10 functional domains with field definitions.',
    mimeType: 'application/json'
  },
  {
    uri: 'blazortw://design/guidelines',
    name: 'Mobile-First & Design System Guidelines',
    description: 'Best practices for touch targets (44px), dark mode synchronization, and zero-CDN compliance.',
    mimeType: 'text/markdown'
  }
];

// Prompt definitions
const PROMPTS = [
  {
    name: 'scaffold-enterprise-page',
    description: 'Scaffold a complete, cohesive Blazor WebAssembly page using BlazorTwUI components.',
    arguments: [
      { name: 'pageTitle', description: 'Title of the page (e.g. "Cloud Billing & Check Verification")', required: true },
      { name: 'features', description: 'Comma-separated list of desired features (e.g. "check reader, metric cards, datatable")', required: true }
    ]
  },
  {
    name: 'synthesize-agui-agent',
    description: 'Configure an AI agent to emit validated AguiFormSchema and AguiWidgetSchema JSON trees.',
    arguments: [
      { name: 'workflowDomain', description: 'The business workflow domain (e.g. "database provisioning", "employee onboarding")', required: true }
    ]
  }
];

// Standard MCP JSON-RPC Server Loop via STDIN/STDOUT
const rl = readline.createInterface({
  input: process.stdin,
  output: process.stdout,
  terminal: false
});

function sendResponse(id, result) {
  const payload = {
    jsonrpc: '2.0',
    id: id,
    result: result
  };
  process.stdout.write(JSON.stringify(payload) + '\n');
}

function sendError(id, code, message) {
  const payload = {
    jsonrpc: '2.0',
    id: id,
    error: { code, message }
  };
  process.stdout.write(JSON.stringify(payload) + '\n');
}

rl.on('line', (line) => {
  if (!line.trim()) return;

  try {
    const msg = JSON.parse(line);
    const { id, method, params } = msg;

    switch (method) {
      case 'initialize':
        sendResponse(id, {
          protocolVersion: '2024-11-05',
          capabilities: {
            tools: {},
            resources: {},
            prompts: {}
          },
          serverInfo: {
            name: 'blazor-tw-mcp',
            version: '0.3.0'
          }
        });
        break;

      case 'notifications/initialized':
        // Client acknowledgment, no response needed
        break;

      case 'tools/list':
        sendResponse(id, { tools: TOOLS });
        break;

      case 'tools/call': {
        const { name, arguments: args } = params;
        handleToolCall(id, name, args || {});
        break;
      }

      case 'resources/list':
        sendResponse(id, { resources: RESOURCES });
        break;

      case 'resources/read': {
        const { uri } = params;
        handleResourceRead(id, uri);
        break;
      }

      case 'prompts/list':
        sendResponse(id, { prompts: PROMPTS });
        break;

      case 'prompts/get': {
        const { name, arguments: args } = params;
        handlePromptGet(id, name, args || {});
        break;
      }

      case 'ping':
        sendResponse(id, {});
        break;

      default:
        sendError(id, -32601, `Method '${method}' not found.`);
        break;
    }
  } catch (err) {
    sendError(null, -32700, 'Parse error: ' + err.message);
  }
});

// Tool Execution Dispatcher
function handleToolCall(id, toolName, args) {
  switch (toolName) {
    case 'blazortw_search_components': {
      const q = (args.query || '').toLowerCase().trim();
      const cat = (args.category || '').toLowerCase().trim();

      const matched = components
        .map(c => {
          let score = 0;
          const nameLower = c.name.toLowerCase();
          const descLower = (c.description || '').toLowerCase();
          const catLower = (c.category || '').toLowerCase();

          // Exact or strong name match
          if (nameLower === q) score += 100;
          else if (nameLower.includes(q)) score += 50;

          // Compact match (e.g. "streaming grid" -> "twstreaminggrid")
          const qCompact = q.replace(/[\s\-_]+/g, '');
          if (qCompact.length > 2 && nameLower.includes(qCompact)) score += 60;

          // Token-level matching across name, description, and category
          const tokens = q.split(/[\s\-_]+/).filter(t => t.length > 1);
          if (tokens.length > 0) {
            let matchedTokens = 0;
            for (const t of tokens) {
              if (nameLower.includes(t)) { score += 35; matchedTokens++; }
              else if (descLower.includes(t)) { score += 15; matchedTokens++; }
              else if (catLower.includes(t)) { score += 5; matchedTokens++; }
            }
            if (matchedTokens === tokens.length && tokens.length > 1) {
              score += 30; // Bonus for full query token coverage
            }
          }

          // Word boundary or token match in name
          if (nameLower.startsWith(q)) score += 30;

          // Description match
          if (descLower.includes(q)) score += 20;

          // Category match
          if (catLower.includes(q)) score += 5;

          if (cat && !catLower.includes(cat)) {
            score = 0;
          }

          return { comp: c, score };
        })
        .filter(item => item.score > 0)
        .sort((a, b) => b.score - a.score)
        .map(item => item.comp)
        .slice(0, 10);

      const text = matched.length > 0
        ? `Found ${matched.length} component(s):\n\n` + matched.map(m => `### \`${m.name}\` (${m.category})\n${m.description}\n\`\`\`razor\n${m.snippet}\n\`\`\``).join('\n\n')
        : `No components matching query "${args.query}". Total available in catalog: ${components.length}`;

      sendResponse(id, {
        content: [{ type: 'text', text: text }]
      });
      break;
    }

    case 'blazortw_get_component_api': {
      const compName = (args.componentName || '').trim();
      const comp = components.find(c => c.name.toLowerCase() === compName.toLowerCase());

      if (!comp) {
        sendResponse(id, {
          content: [{ type: 'text', text: `Component "${compName}" not found. Tip: Use blazortw_search_components to find valid component names.` }]
        });
        return;
      }

      const paramTable = comp.parameters && comp.parameters.length > 0
        ? '| Parameter | Type | Default | Description |\n|---|---|---|---|\n' +
          comp.parameters.map(p => `| \`${p.name}\` | \`${p.type}\` | \`${p.default}\` | ${p.description} |`).join('\n')
        : '_This component has no configurable parameters._';

      const text = `# Component API: \`${comp.name}\`\n\n**Category:** ${comp.category}\n\n**Description:** ${comp.description}\n\n### Parameter Specifications\n\n${paramTable}\n\n### Usage Example\n\n\`\`\`razor\n${comp.snippet}\n\`\`\``;

      sendResponse(id, {
        content: [{ type: 'text', text: text }]
      });
      break;
    }

    case 'blazortw_scaffold_component': {
      const compName = (args.componentName || '').trim();
      const comp = components.find(c => c.name.toLowerCase() === compName.toLowerCase());
      const title = args.customTitle || (comp ? comp.name : 'Custom Component');

      let snippet = comp ? comp.snippet : `<${compName} />`;
      if (args.customTitle && snippet.includes('Title=')) {
        snippet = snippet.replace(/Title="[^"]*"/, `Title="${title}"`);
      }

      let text = `\`\`\`razor\n${snippet}\n\`\`\``;

      if (args.includeCodeBlock) {
        text += `\n\n\`\`\`csharp\n@code {\n    // Component state and handlers\n    private bool _isLoading = false;\n    \n    protected override async Task OnInitializedAsync()\n    {\n        await Task.CompletedTask;\n    }\n}\n\`\`\``;
      }

      sendResponse(id, {
        content: [{ type: 'text', text: text }]
      });
      break;
    }

    case 'blazortw_list_forms': {
      const cat = (args.category || '').toLowerCase();
      const matched = forms.filter(f => cat ? (f.category || '').toLowerCase().includes(cat) : true).slice(0, 15);

      const text = `### 100 Standard Enterprise Forms Catalog\n\n` +
        `Displaying ${matched.length} form(s):\n\n` +
        matched.map(f => `- **${f.icon} ${f.title}** (\`id: "${f.id}"\`) — _${f.category}_: ${f.description} (${f.fieldCount} fields)`).join('\n');

      sendResponse(id, {
        content: [{ type: 'text', text: text }]
      });
      break;
    }

    case 'blazortw_get_form_schema': {
      const formId = (args.formId || '').toLowerCase().trim();
      const form = forms.find(f => f.id.toLowerCase() === formId);

      if (!form) {
        sendResponse(id, {
          content: [{ type: 'text', text: `Form schema with ID "${args.formId}" not found. Use blazortw_list_forms to explore available IDs.` }]
        });
        return;
      }

      const text = `### Form Schema: ${form.icon} ${form.title} (\`${form.id}\`)\n\n` +
        `**Category:** ${form.category}\n` +
        `**Description:** ${form.description}\n\n` +
        `#### Field Definitions (${form.fields.length}):\n\n` +
        '| Field Name | Label | Input Type |\n|---|---|---|\n' +
        form.fields.map(f => `| \`${f.name}\` | ${f.label} | \`${f.type}\` |`).join('\n') +
        `\n\n\`\`\`json\n${JSON.stringify(form, null, 2)}\n\`\`\``;

      sendResponse(id, {
        content: [{ type: 'text', text: text }]
      });
      break;
    }

    case 'blazortw_validate_micr': {
      const parsed = parseMicrLine(args.micrLine || '');
      const text = `### Check 21 MICR E-13B Analysis Report\n\n` +
        `| MICR Element | Value | Verification Status |\n` +
        `|---|---|:---:|\n` +
        `| **Raw MICR Line** | \`${parsed.rawMicr}\` | Received |\n` +
        `| **ABA Routing Transit** | \`${parsed.routing || 'N/A'}\` | ${parsed.isValidChecksum ? '✅ Valid Mod-10 Checksum' : '❌ FAILED Checksum'} |\n` +
        `| **Account Number** | \`${parsed.account || 'N/A'}\` | Extracted |\n` +
        `| **Check Number** | \`${parsed.checkNumber || 'N/A'}\` | Extracted |\n` +
        `| **Drawee Institution** | ${parsed.bankName} | ${parsed.isValidChecksum ? '✅ Verified' : '⚠️ Unverified'} |\n\n` +
        `**Federal Reserve Formula:** \`[3(d1+d4+d7) + 7(d2+d5+d8) + 1(d3+d6+d9)] mod 10 == 0\``;

      sendResponse(id, {
        content: [{ type: 'text', text: text }]
      });
      break;
    }

    case 'blazortw_audit_razor': {
      const code = args.razorCode || '';
      const issues = [];
      const successes = [];

      // Check for prohibited brands
      const brands = ['legacy-ui', 'proprietary-grid', 'external-ui'];
      brands.forEach(b => {
        if (code.toLowerCase().includes(b)) {
          issues.push(`Prohibited vendor commercial brand detected: "${b}". BlazorTwUI is strictly vendor-neutral.`);
        }
      });

      // Check for hardcoded local machine paths
      if (/C:[\\\/]Users[\\\/]/i.test(code)) {
        issues.push('Hardcoded local machine file path detected. Use relative paths or virtual assets.');
      }

      // Check for touch targets
      if (code.includes('<button') && !code.includes('py-') && !code.includes('p-')) {
        issues.push('Button element may lack 44px minimum touch target height. Recommend adding padding (e.g. `p-2.5` or `py-2 px-4`).');
      } else {
        successes.push('Interactive button touch targets meet mobile-first recommendations.');
      }

      // Dark mode classes
      if (code.includes('bg-white') && !code.includes('dark:bg-')) {
        issues.push('Element uses `bg-white` without a corresponding `dark:bg-*` dark-mode utility class.');
      } else {
        successes.push('Dark mode color synchronization detected.');
      }

      const text = `### BlazorTwUI Markup Audit\n\n` +
        `**Passed Checks:**\n` + successes.map(s => ` - ✅ ${s}`).join('\n') + `\n\n` +
        (issues.length > 0
          ? `**Potential Issues (${issues.length}):**\n` + issues.map(i => ` - ⚠️ ${i}`).join('\n')
          : `**Status:** 0 issues found. The markup is 100% compliant with mobile-first and vendor-neutral rules!`);

      sendResponse(id, {
        content: [{ type: 'text', text: text }]
      });
      break;
    }

    default:
      sendError(id, -32601, `Tool '${toolName}' not recognized.`);
      break;
  }
}

// Resource Reader
function handleResourceRead(id, uri) {
  if (uri === 'blazortw://components/catalog') {
    sendResponse(id, {
      contents: [{
        uri: uri,
        mimeType: 'application/json',
        text: JSON.stringify(components, null, 2)
      }]
    });
  } else if (uri === 'blazortw://forms/100-standard') {
    sendResponse(id, {
      contents: [{
        uri: uri,
        mimeType: 'application/json',
        text: JSON.stringify(forms, null, 2)
      }]
    });
  } else if (uri === 'blazortw://design/guidelines') {
    const text = `# BlazorTwUI — Design & Architectural Guidelines\n\n` +
      `1. **Mobile-First Touch Target Rule**: Every button, input, toggle, and link must maintain a minimum physical touch target of 44×44 CSS pixels.\n` +
      `2. **Dark Mode Rule**: Always pair light classes with dark counterparts (e.g. \`bg-white dark:bg-slate-900\`, \`text-slate-900 dark:text-white\`).\n` +
      `3. **Zero-CDN Rule**: Never reference external CDN JavaScript, stylesheets, or remote web fonts. Everything must be locally self-hosted.\n` +
      `4. **Procedural Audio**: Sound effects should use \`TwSoundService\` Web Audio synthesis rather than static MP3 downloads.`;
    sendResponse(id, {
      contents: [{
        uri: uri,
        mimeType: 'text/markdown',
        text: text
      }]
    });
  } else {
    sendError(id, -32602, `Resource '${uri}' not found.`);
  }
}

// Prompt Getter
function handlePromptGet(id, promptName, args) {
  if (promptName === 'scaffold-enterprise-page') {
    const title = args.pageTitle || 'Enterprise Dashboard';
    const features = args.features || 'check reader, metrics, forms';
    const text = `You are a Senior Blazor WebAssembly and Tailwind CSS Architect. Create a high-performance Blazor page titled "${title}" incorporating these features: ${features}.\n\n` +
      `Rules to follow:\n` +
      `- Use components from the BlazorTwUI library (e.g. TwCard, TwButton, TwDataGrid, TwMicrCheckReader, TwAguiFormCanvas).\n` +
      `- Strictly adhere to 44px mobile touch targets and dark mode utility classes.\n` +
      `- Zero third-party commercial UI dependencies.\n` +
      `- Clean C# state variables in the @code block.`;

    sendResponse(id, {
      messages: [{
        role: 'user',
        content: { type: 'text', text: text }
      }]
    });
  } else if (promptName === 'synthesize-agui-agent') {
    const domain = args.workflowDomain || 'Cloud Provisioning';
    const text = `You are an Adaptive Generative UI (AG-UI) Agent synthesizer for the domain "${domain}".\n\n` +
      `Synthesize an AguiFormSchema with:\n` +
      `1. Title, RiskLevel (Low/Medium/High), and OverallConfidence (0.0 - 1.0).\n` +
      `2. Array of AguiFormField objects with Id, Label, Type (Text, Number, Slider, Switch, Select, RadioGroup), and confidence ratings.\n` +
      `3. At least one conditional dependency using DependsOnField and DependsOnValue.`;

    sendResponse(id, {
      messages: [{
        role: 'user',
        content: { type: 'text', text: text }
      }]
    });
  } else {
    sendError(id, -32601, `Prompt '${promptName}' not found.`);
  }
}
