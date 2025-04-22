import React, { useState } from 'react';
import { Box,   Typography,  Button, FormControl, MenuItem, Select, SelectChangeEvent } from '@mui/material';

import UploadFile from '@mui/icons-material/UploadFile';
import { useDropzone } from 'react-dropzone';
import OcrResultDisplay from '../components/OcrResultDisplay';
import QualityAssessmentDisplay from '../components/QualityAssessmentDisplay';
import DocumentService, {OcrDocumentResponse} from '../services/DocumentService';



const MainScreen: React.FC = () => {

    const [imageSrc, setImageSrc] = useState<string | null>(null);
    const [documentType, setDocumentType] = useState<string>('');
    const [uploadedFile, setUploadedFile] = useState<File | null>(null);
    const [loading, setLoading] = useState(false);
    const [ocrResult, setOcrResult] = useState<OcrDocumentResponse | null>(null);



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
        try {
            setLoading(true);
            const response = await DocumentService.ocrDocument(uploadedFile, documentType);
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
                            <Select
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
                    <Button
                        variant="contained"
                        color="secondary"
                        disabled={!documentType}
                        onClick = {handleOcrDocument}
                        sx={{
                            marginTop: '16px',
                            width: '100%', // Full width of the parent container
                        }}
                    >
                        OCR {documentType}
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