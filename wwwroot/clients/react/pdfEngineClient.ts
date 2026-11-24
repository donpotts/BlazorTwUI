/**
 * Cross-Platform TypeScript / React Client for BlazorTailwindUI Pure C# PDF Engine.
 * 100% Free, Zero-Dependency, MIT License.
 * Works seamlessly with Next.js, Vite, React SPA, Node.js, and Express.
 */

export type PdfFieldType = 'SignHere' | 'InitialHere' | 'DateTimeStamp' | 'Text' | 'Checkbox' | 'Dropdown';
export type PdfPageSize = 'Letter' | 'A4' | 'Legal' | 'Ledger' | 'Custom';
export type BatesPosition = 'BottomRight' | 'BottomCenter' | 'BottomLeft' | 'TopRight' | 'TopCenter' | 'TopLeft';

export interface PdfWatermarkOptions {
  enabled: boolean;
  text: string;
  opacity?: number;
  rotationDegrees?: number;
  fontSize?: number;
  colorRgb?: [number, number, number];
}

export interface PdfBatesOptions {
  enabled: boolean;
  prefix: string;
  startNumber?: number;
  digits?: number;
  suffix?: string;
  position?: BatesPosition;
}

export interface PdfContractField {
  id: string;
  type: PdfFieldType;
  label: string;
  signerRole?: string;
  page: number;
  x?: number;
  y?: number;
  width?: number;
  height?: number;
  value?: string;
  isRequired?: boolean;
  isReadOnly?: boolean;
  signedAt?: string;
  signerName?: string;
  signatureDataUrl?: string;
  signatureStyle?: string;
  placeholder?: string;
}

export interface PdfContractSection {
  heading: string;
  paragraphs: string[];
}

export interface PdfContractPage {
  pageNumber: number;
  title: string;
  subtitle?: string;
  pageSize?: PdfPageSize;
  sections: PdfContractSection[];
}

export interface PdfContractDocument {
  documentId: string;
  title: string;
  contractType?: string;
  effectiveDate: string;
  status?: string;
  author?: string;
  subject?: string;
  keywords?: string;
  creator?: string;
  isReadOnly?: boolean;
  isSealed?: boolean;
  isFlattened?: boolean;
  sha256Fingerprint?: string;
  certificateId?: string;
  signerIp?: string;
  defaultPageSize?: PdfPageSize;
  watermark?: PdfWatermarkOptions;
  batesNumbering?: PdfBatesOptions;
  pages: PdfContractPage[];
  fields: PdfContractField[];
}

export interface PdfGenerateOptions {
  flatten?: boolean;
  applyWatermark?: boolean;
  watermarkText?: string;
  applyBates?: boolean;
  batesPrefix?: string;
  returnBase64?: boolean;
}

export class PdfEngineClient {
  private baseUrl: string;

  constructor(baseUrl: string = '/api/pdf') {
    this.baseUrl = baseUrl.replace(/\/$/, '');
  }

  /**
   * Generates a binary PDF Blob directly from the .NET Minimal API backend.
   */
  async generateBlob(doc: PdfContractDocument, options: PdfGenerateOptions = {}): Promise<Blob> {
    const res = await fetch(`${this.baseUrl}/generate`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        document: doc,
        flatten: options.flatten ?? false,
        applyWatermark: options.applyWatermark ?? false,
        watermarkText: options.watermarkText,
        applyBates: options.applyBates ?? false,
        batesPrefix: options.batesPrefix,
        returnBase64: false
      })
    });

    if (!res.ok) {
      throw new Error(`PDF generation failed: ${res.statusText}`);
    }

    return await res.blob();
  }

  /**
   * Triggers an immediate browser file download for the generated PDF.
   */
  async downloadPdf(doc: PdfContractDocument, filename?: string, options: PdfGenerateOptions = {}): Promise<void> {
    const blob = await this.generateBlob(doc, options);
    const fname = filename || `${doc.documentId || 'Document'}.pdf`;
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fname;
    document.body.appendChild(a);
    a.click();
    setTimeout(() => {
      document.body.removeChild(a);
      window.URL.revokeObjectURL(url);
    }, 1000);
  }

  /**
   * Flattens and purges AcroForms into an immutable static PDF stream.
   */
  async flattenAndDownload(doc: PdfContractDocument, filename?: string): Promise<void> {
    const res = await fetch(`${this.baseUrl}/flatten`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ document: doc, returnBase64: false })
    });
    if (!res.ok) throw new Error(`PDF flattening failed: ${res.statusText}`);

    const blob = await res.blob();
    const fname = filename || `${doc.documentId}_Flattened.pdf`;
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fname;
    document.body.appendChild(a);
    a.click();
    setTimeout(() => {
      document.body.removeChild(a);
      window.URL.revokeObjectURL(url);
    }, 1000);
  }

  /**
   * Combines multiple documents into a single multi-page unified PDF.
   */
  async combineAndDownload(docs: PdfContractDocument[], title: string = 'Portfolio', batesPrefix = 'BATES-'): Promise<void> {
    const res = await fetch(`${this.baseUrl}/combine`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        title,
        documents: docs,
        flattenAll: true,
        applyContinuousBates: true,
        batesPrefix,
        returnBase64: false
      })
    });
    if (!res.ok) throw new Error(`PDF combine failed: ${res.statusText}`);

    const blob = await res.blob();
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `${title.replace(/\s+/g, '_')}.pdf`;
    document.body.appendChild(a);
    a.click();
    setTimeout(() => {
      document.body.removeChild(a);
      window.URL.revokeObjectURL(url);
    }, 1000);
  }

  /**
   * Fetches an enterprise document template from the backend factory.
   */
  async getTemplate(type: 'msa' | 'invoice' | 'nda' | 'certificate'): Promise<PdfContractDocument> {
    const res = await fetch(`${this.baseUrl}/templates/${type}`);
    if (!res.ok) throw new Error(`Failed to load template '${type}': ${res.statusText}`);
    return await res.json();
  }

  /**
   * Inspects PDF document capabilities and server health.
   */
  async getCapabilities(): Promise<any> {
    const res = await fetch(`${this.baseUrl}/health`);
    return await res.json();
  }
}
