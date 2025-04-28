import React, { useState, useEffect } from 'react';
import { Box,   Typography,  Button, FormControl, MenuItem, Select, SelectChangeEvent, InputLabel, Stack } from '@mui/material';

import UploadFile from '@mui/icons-material/UploadFile';
import { useDropzone } from 'react-dropzone';
import OcrResultDisplay from '../components/OcrResultDisplay';
import QualityAssessmentDisplay from '../components/QualityAssessmentDisplay';
import DocumentService, {OcrDocumentResponse, OCRService, LLMService, LLMModel} from '../services/DocumentService';



const MainScreen: React.FC = () => {

    const [imageSrc, setImageSrc] = useState<string | null>(null);
    const [documentType, setDocumentType] = useState<string>('');
    const [uploadedFile, setUploadedFile] = useState<File | null>(null);
    const [ocrProvider, setOcrProvider] = useState<string>('');
    const [llm, setLlm] = useState<string>('');
    const [selectedModel, setSelectedModel] = useState<string>('');
    const [loading, setLoading] = useState(false);
    const [ocrResult, setOcrResult] = useState<OcrDocumentResponse | null>(null);
    const [ocrServices, setOcrServices] = useState<OCRService[]>([]);
    const [llmServices, setLlmServices] = useState<LLMService[]>([]);
    const [llmModels, setLlmModels] = useState<LLMModel[]>([]);



    useEffect(() => {
        // Fetch OCR and LLM services when on mount
        const fetchServices = async () => {
            try {
                const [ocrServicesData, llmServicesData] = await Promise.all([
                    DocumentService.getOCRServices(),
                    DocumentService.getLLMServices()
                ]);
                
                setOcrServices(ocrServicesData);
                setLlmServices(llmServicesData);
                
                // Set default values if available
                if (ocrServicesData.length > 0) {
                    setOcrProvider(ocrServicesData[0].ServiceId);
                }
                
                if (llmServicesData.length > 0) {
                    // Find the default model if it exists
                    const defaultService = llmServicesData[0];
                    setLlm(defaultService.ServiceId);
                    
                    // Set available models for the selected LLM service
                    setLlmModels(defaultService.Models);
                    
                    // Set default model if available
                    if (defaultService.Models && defaultService.Models.length > 0) {
                        // Try to find a default model first
                        const defaultModel = defaultService.Models.find(model => model.IsDefault);
                        if (defaultModel) {
                            setSelectedModel(defaultModel.Name);
                        } else if (defaultService.Models.length > 0) {
                            // Otherwise use the first model
                            setSelectedModel(defaultService.Models[0].Name);
                        }
                    }
                }
            } catch (error) {
                console.error("Error fetching services:", error);
            }
        };
        
        fetchServices();
    }, []);
    
    const onDrop = (acceptedFiles: File[]) => {
        const file = acceptedFiles[0];
        if (file) {
            setUploadedFile(file);
            setImageSrc(null); // Clear the current image
            setDocumentType(''); // Reset the document type
            setOcrResult(null); // Clear the OCR result display
            const reader = new FileReader();
            reader.onload = () => {
                setImageSrc(reader.result as string);
            };
            reader.readAsDataURL(file);
        }
    };

    const handleDocumentTypeChange = (event: SelectChangeEvent) => {
        setDocumentType(event.target.value);
    };
    
    const handleOcrProviderChange = (event: SelectChangeEvent) => {
        setOcrProvider(event.target.value);
    };
    
    const handleLlmChange = (event: SelectChangeEvent) => {
        const selectedLlmType = event.target.value;
        setLlm(selectedLlmType);
        
        // Update models when LLM service changes
        const selectedService = llmServices.find(service => service.ServiceId === selectedLlmType);
        if (selectedService) {
            setLlmModels(selectedService.Models);
            
            // Set default model if available
            if (selectedService.Models && selectedService.Models.length > 0) {
                // Try to find a default model first
                const defaultModel = selectedService.Models.find(model => model.IsDefault);
                if (defaultModel) {
                    setSelectedModel(defaultModel.Name);
                } else {
                    // Otherwise use the first model
                    setSelectedModel(selectedService.Models[0].Name);
                }
            } else {
                setSelectedModel('');
            }
        } else {
            setLlmModels([]);
            setSelectedModel('');
        }
    };
    
    const handleModelChange = (event: SelectChangeEvent) => {
        setSelectedModel(event.target.value);
    };

    const handleIdentifyDocument = async () => {
        if (!uploadedFile) {
            alert("Please upload a file first.");
            return;
        }
        try {
            const result = await DocumentService.identifyDocument(uploadedFile);
            console.warn(result);
            setDocumentType(result.DocumentType);
        } catch (error) {
            console.error("Error identifying document:", error);
            alert("Failed to identify document.");
        }
    };

    const { getRootProps, getInputProps } = useDropzone({
        onDrop,
        accept: { 'image/*': [] },
        multiple: false,
    });


    // Assuming you have a file input, a document type selector, and an OCR button in your UI



    const handleOcrDocument = async () => {

        if (!documentType) {
            console.error("No document type selected.");
            return;
        }
        if (!uploadedFile) {
            console.error("No file uploaded.");
            return;
        }
        if (!ocrProvider) {
            console.error("No OCR provider selected.");
            return;
        }
        try {
            setLoading(true);
            const response = await DocumentService.ocrDocument(uploadedFile, documentType, llm, ocrProvider);
            setOcrResult(response);
            console.log("OCR Response:", response);
        } catch (error) {
            setOcrResult(null);
            console.error("Failed to OCR document:", error);
        }
        finally {
            setLoading(false);
        }
    };
    
    return (
        <Box
            sx={{
                display: 'flex',
                height: '100vh', // Constrain to viewport height
                overflow: 'hidden', // Prevent scrolling on the browser window
            }}
        >





            {/* Main content area */}
            <Box
                component="main"
                sx={{
                    flexGrow: 1,
                    display: 'grid',
                    gridTemplateColumns: '400px 1fr 1fr',
                    gap: '16px',
                    height: '100vh', // Constrain to viewport height
                    overflow: 'hidden', // Prevent content overflow
                }}
            >



                {/* First column: Image and text */}
                <Box
                    sx={{
                        display: 'flex',
                        flexDirection: 'column', // Stack items vertically
                        alignItems: 'center', // Center items horizontally
                        width: '100%', // Full width of the column
                        height: '100%', // Full height of the column
                        overflow: 'hidden',
                        border: '1px solid #ccc',
                        borderRadius: '8px',
                        backgroundColor: '#fff',
                        padding: '16px', // Add padding for spacing
                        gap: '16px', // Add spacing between items
                    }}
                >

                    <Box
                        {...getRootProps()}
                        sx={{
                            width: '200px',
                            height: '200px',
                            border: '2px dashed #ccc',
                            borderRadius: '8px',
                            padding: '16px',
                            textAlign: 'center',
                            cursor: 'pointer',
                            backgroundColor: '#f9f9f9',
                            display: 'flex',
                            flexDirection: 'column',
                            justifyContent: 'space-between',
                            alignItems: 'center',
                        }}
                    >
                        <input {...getInputProps()} />
                        <UploadFile sx={{ fontSize: '80px', color: '#ccc' }} />
                        <Typography variant="body1" sx={{ alignSelf: 'flex-end' }}>
                            Drop an image here
                        </Typography>
                    </Box>
                    {/* Image */}
                    {imageSrc && (
                        <img
                            src={imageSrc}
                            alt="Dropped file"
                            style={{
                                width: '100%',
                                aspectRatio: '1.29',
                                minHeight: '50px',
                                objectFit: 'contain',
                            }}
                        />
                    )}

                    {/* Button and Selector */}
                    <Box
                        sx={{
                            display: 'flex',
                            flexDirection: 'row', // Align button and selector horizontally
                            justifyContent: 'space-between',
                            alignItems: 'center',
                            width: '100%', // Full width of the parent container
                        }}
                    >
                        <Button variant="contained" color="primary" onClick={handleIdentifyDocument}>
                            Identify
                        </Button>

                        <FormControl sx={{ flexGrow: 1, marginLeft: '16px' }}>
                            <InputLabel id="document-type-label">Document Type</InputLabel>
                            <Select
                                label="Document Type"
                                labelId="document-type-label"
                                value={documentType}
                                onChange={handleDocumentTypeChange}
                            >
                                <MenuItem value="">
                                    <em>None</em>
                                </MenuItem>
                                <MenuItem value="Bill of Lading">Bill of Lading</MenuItem>
                                <MenuItem value="Rate Confirmation">Rate Confirmation</MenuItem>
                                <MenuItem value="Invoice">Invoice</MenuItem>
                                <MenuItem value="Fuel Receipt">Fuel Receipt</MenuItem>
                            </Select>
                        </FormControl>
                    </Box>
                    
                    
                    <Stack direction="row" spacing={2} sx={{ width: '100%', marginTop: '16px' }}>


                        <FormControl sx={{ flexGrow: 1, marginLeft: '16px' }}>
                            <InputLabel id="ocr-provider-label">Ocr Provider</InputLabel>
                            <Select
                                label="Ocr Provider"
                                labelId="ocr-provider-label"
                                value={ocrProvider}
                                onChange={handleOcrProviderChange}
                            >
                                <MenuItem value="None - Use LLM">
                                    <em>None - Use LLM</em>
                                </MenuItem>
                                {ocrServices.map((service) => (
                                    <MenuItem key={service.ServiceId} value={service.ServiceId}>
                                        {service.Name}
                                    </MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                        <FormControl sx={{ flexGrow: 1, marginLeft: '16px' }}>
                            <InputLabel id="llm-label">LLM</InputLabel>
                            <Select
                                label="LLM"
                                labelId="llm-label"
                                value={llm}
                                onChange={handleLlmChange}
                            >
                                <MenuItem value="">
                                    <em>None</em>
                                </MenuItem>
                                {llmServices.map((service) => (
                                    <MenuItem key={service.ServiceId} value={service.ServiceId}>
                                        {service.Name}
                                    </MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                                            </Stack>
                                            
                                            {/* Model selector - only show when an LLM is selected */}
                                            {llm && llmModels.length > 0 && (
                        <FormControl sx={{ width: '100%' }}>
                            <InputLabel id="model-label">Model</InputLabel>
                            <Select
                                label="Model"
                                labelId="model-label"
                                value={selectedModel}
                                onChange={handleModelChange}
                            >
                                {llmModels.map((model) => (
                                    <MenuItem 
                                        key={model.Name} 
                                        value={model.Name}
                                    >
                                        {model.Name} {model.IsDefault && '(Default)'}
                                    </MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                                            )}
                    <Button
                        variant="contained"
                        color="secondary"
                        disabled={!documentType}
                        onClick={handleOcrDocument}
                        sx={{
                            marginTop: '16px',
                            width: '100%', // Full width of the parent container
                        }}
                    >
                        OCR {documentType} {selectedModel ? `using ${selectedModel}` : ''}
                    </Button>
                </Box>



                {/* Third column: Placeholder */}
                <Box
                    sx={{

                        display: 'flex',
                        justifyContent: 'center',
                        alignItems: 'flex-start', // Align content to the top
                        height: '100%',
                        width: '100%',
                        overflow: 'auto', // Allow independent scrolling
                        padding: '16px', // Add padding to prevent cutting off
                    }}
                >
                    <OcrResultDisplay loading={loading} ocrResult={ocrResult} documentType={documentType} />
                </Box>

                <Box
                    sx={{

                        display: 'flex',
                        justifyContent: 'center',
                        alignItems: 'flex-start', // Align content to the top
                        height: '100%',
                        width: '100%',
                        overflow: 'auto', // Allow independent scrolling
                        padding: '16px', // Add padding to prevent cutting off
                    }}
                >
                    <QualityAssessmentDisplay qualityAssessment={ocrResult?.QualityAssessment || null} />
                </Box>
                
            </Box>
        </Box>
    );
};

export default MainScreen;