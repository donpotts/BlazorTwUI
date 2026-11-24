/**
 * React Native & Expo Adapter for BlazorTailwindUI Pure C# PDF Engine.
 * 100% Free, Zero-Dependency, MIT License.
 * Uses standard fetch with expo-file-system and expo-sharing to save and view PDFs on iOS & Android.
 */

import { PdfContractDocument, PdfGenerateOptions } from '../react/pdfEngineClient';

export interface ExpoFileSystemLike {
  documentDirectory: string | null;
  writeAsStringAsync(fileUri: string, contents: string, options?: any): Promise<void>;
  EncodingType: {
    Base64: string;
  };
}

export interface ExpoSharingLike {
  isAvailableAsync(): Promise<boolean>;
  shareAsync(url: string, options?: any): Promise<void>;
}

export class ExpoPdfEngine {
  private apiBaseUrl: string;

  constructor(apiBaseUrl: string) {
    this.apiBaseUrl = apiBaseUrl.replace(/\/$/, '');
  }

  /**
   * Generates Base64 PDF payload from backend Minimal API.
   */
  async generateBase64(doc: PdfContractDocument, options: PdfGenerateOptions = {}): Promise<string> {
    const res = await fetch(`${this.apiBaseUrl}/generate`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        document: doc,
        flatten: options.flatten ?? false,
        applyWatermark: options.applyWatermark ?? false,
        watermarkText: options.watermarkText,
        applyBates: options.applyBates ?? false,
        batesPrefix: options.batesPrefix,
        returnBase64: true
      })
    });

    if (!res.ok) {
      throw new Error(`Mobile PDF generation failed: ${res.statusText}`);
    }

    const data = await res.json();
    return data.base64Data;
  }

  /**
   * Saves PDF locally into the mobile device's sandbox and invokes native OS share/preview sheet.
   */
  async saveAndShareOnDevice(
    doc: PdfContractDocument,
    fileSystem: ExpoFileSystemLike,
    sharing: ExpoSharingLike,
    customFilename?: string,
    options: PdfGenerateOptions = {}
  ): Promise<string> {
    const base64 = await this.generateBase64(doc, options);
    const fname = customFilename || `${doc.documentId || 'Contract'}.pdf`;
    const targetUri = `${fileSystem.documentDirectory}${fname}`;

    await fileSystem.writeAsStringAsync(targetUri, base64, {
      encoding: fileSystem.EncodingType.Base64
    });

    if (await sharing.isAvailableAsync()) {
      await sharing.shareAsync(targetUri, {
        mimeType: 'application/pdf',
        dialogTitle: `View ${doc.title || 'PDF Document'}`
      });
    }

    return targetUri;
  }
}
