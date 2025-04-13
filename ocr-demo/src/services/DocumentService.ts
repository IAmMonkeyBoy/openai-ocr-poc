// Define interfaces for API responses
export interface IdentifyDocumentResponse {
    FileName: string;
    DocumentType: string;
}

export interface OcrDocumentResponse {
    FileName: string;
    DocumentType: string;
    OcrText: string;
}

const API_BASE_URL = "http://localhost:5194/";

class DocumentService {
    static async identifyDocument(file: File): Promise<IdentifyDocumentResponse> {
        const formData = new FormData();
        formData.append("file", file);

        const response = await fetch(`${API_BASE_URL}identify-document`, {
            method: "POST",
            body: formData,
        });

        if (!response.ok) {
            throw new Error("Failed to identify document.");
        }

        return response.json() as Promise<IdentifyDocumentResponse>;
    }

    static async ocrDocument(file: File, documentType: string): Promise<OcrDocumentResponse> {
        const formData = new FormData();
        formData.append("file", file);

        const response = await fetch(`${API_BASE_URL}ocr-document/${documentType}`, {
            method: "POST",
            body: formData,
        });

        if (!response.ok) {
            throw new Error("Failed to OCR document.");
        }

        return response.json() as Promise<OcrDocumentResponse>;
    }
}

export default DocumentService;