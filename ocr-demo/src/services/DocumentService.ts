// Define interfaces for API responses
export interface IdentifyDocumentResponse {
    FileName: string;
    DocumentType: string;
}

export interface OcrDocumentResponse {
 Document: string;
QualityAssessment: QualityAssessment;
}


export interface ModelProvider {
    SupportedModels: string[];
    ProviderName: string;
    ProviderDisplayName: string;
    ServiceType: string;
}
export interface QualityAssessment{
    FieldEvaluations: FieldEvaluation[];
}

export interface FieldEvaluation
{
    FieldName: string;
    IsPresent: boolean;
    Priority: "High" | "Medium" | "Low" | "None";
    Value: string;
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

        return await response.json() as Promise<IdentifyDocumentResponse>;
    }


    
    static async getAvailableModels(): Promise<ModelProvider[]> {
        const response = await fetch(`${API_BASE_URL}models`, {
            method: "GET",
        });
    
        if (!response.ok) {
            throw new Error("Failed to fetch available models.");
        }
    
        return await response.json() as ModelProvider[];
    }
    
    static async ocrDocument(file: File, documentType: string, model?: string): Promise<OcrDocumentResponse> {
            const formData = new FormData();
            formData.append("file", file);
            
            // Add model to request if provided
            const url = model 
                ? `${API_BASE_URL}ocr-document/${documentType}?model=${encodeURIComponent(model)}`
                : `${API_BASE_URL}ocr-document/${documentType}`;
    
            const response = await fetch(url, {
            method: "POST",
            body: formData,
        });

        if (!response.ok) {
            throw new Error("Failed to OCR document.");
        }

        return await response.json() as Promise<OcrDocumentResponse>;
    }
}

export default DocumentService;