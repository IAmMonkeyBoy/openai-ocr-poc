// Define interfaces for API responses
export interface IdentifyDocumentResponse {
    FileName: string;
    DocumentType: string;
}

export interface OcrDocumentResponse {
 Document: string;
 QualityAssessment: QualityAssessment;
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


export interface LLMService 
{
    Name: string;
    Description: string;
    ServiceId: string;
    Models: LLMModel[];
}
export interface LLMModel
{
    Name: string;
    Description: string;
    ModelType: string;
    IsDefault: boolean;
}

export interface OCRService 
{
    Name: string;
    Description: string;
    ServiceId: string;

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

    static async ocrDocument(file: File, documentType: string, llmServiceId: string, ocrServiceId: string ): Promise<OcrDocumentResponse> {
        const formData = new FormData();
        formData.append("file", file);
        formData.append("documentType", documentType);
        formData.append("structuredDocumentServiceId", llmServiceId);
        formData.append("ocrServiceId", ocrServiceId);

        const response = await fetch(`${API_BASE_URL}ocr-document/${documentType}`, {
            method: "POST",
            body: formData,
        });

        if (!response.ok) {
            throw new Error("Failed to OCR document.");
        }

        return await response.json() as Promise<OcrDocumentResponse>;
    }
    
    
    static async getLLMServices(): Promise<LLMService[]> {
        const response = await fetch(`${API_BASE_URL}llm-services`);

        if (!response.ok) {
            throw new Error("Failed to fetch LLM services.");
        }

        return await response.json() as Promise<LLMService[]>;
    }
    
    static async getOCRServices(): Promise<OCRService[]> {
        const response = await fetch(`${API_BASE_URL}ocr-services`);

        if (!response.ok) {
            throw new Error("Failed to fetch OCR services.");
        }

        return await response.json() as Promise<OCRService[]>;
    }
}

export default DocumentService;