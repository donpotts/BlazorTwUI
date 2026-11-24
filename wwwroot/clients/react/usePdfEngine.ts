/**
 * React Hook for BlazorTailwindUI Pure C# PDF Engine.
 * Enables zero-boilerplate state management, real-time signing, flattening, and downloading in React.
 */

import { useState, useCallback } from 'react';
import { PdfEngineClient, PdfContractDocument, PdfGenerateOptions } from './pdfEngineClient';

export function usePdfEngine(apiBaseUrl: string = '/api/pdf') {
  const [client] = useState(() => new PdfEngineClient(apiBaseUrl));
  const [document, setDocument] = useState<PdfContractDocument | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadTemplate = useCallback(async (type: 'msa' | 'invoice' | 'nda' | 'certificate') => {
    setLoading(true);
    setError(null);
    try {
      const doc = await client.getTemplate(type);
      setDocument(doc);
      return doc;
    } catch (err: any) {
      setError(err?.message || 'Failed to load template');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [client]);

  const updateFieldValue = useCallback((fieldId: string, value: string, signerName?: string) => {
    setDocument((prev) => {
      if (!prev) return null;
      return {
        ...prev,
        fields: prev.fields.map((f) => {
          if (f.id === fieldId) {
            return {
              ...f,
              value,
              signerName: signerName ?? f.signerName,
              signedAt: new Date().toISOString()
            };
          }
          return f;
        })
      };
    });
  }, []);

  const downloadPdf = useCallback(async (filename?: string, options: PdfGenerateOptions = {}) => {
    if (!document) throw new Error('No active document to generate');
    setLoading(true);
    setError(null);
    try {
      await client.downloadPdf(document, filename, options);
    } catch (err: any) {
      setError(err?.message || 'Download failed');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [client, document]);

  const flattenAndDownload = useCallback(async (filename?: string) => {
    if (!document) throw new Error('No active document to flatten');
    setLoading(true);
    setError(null);
    try {
      await client.flattenAndDownload(document, filename);
      setDocument((prev) => (prev ? { ...prev, isFlattened: true, isReadOnly: true } : null));
    } catch (err: any) {
      setError(err?.message || 'Flatten failed');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [client, document]);

  return {
    document,
    setDocument,
    loading,
    error,
    loadTemplate,
    updateFieldValue,
    downloadPdf,
    flattenAndDownload,
    client
  };
}
