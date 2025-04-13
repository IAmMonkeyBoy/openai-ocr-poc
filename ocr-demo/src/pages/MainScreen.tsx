import React, { useState } from 'react';
import { Box, Drawer, Toolbar, Typography, IconButton, Button, FormControl, InputLabel, MenuItem, Select, SelectChangeEvent } from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import UploadFile from '@mui/icons-material/UploadFile';
import { useDropzone } from 'react-dropzone';
import FirstColumn from '../components/FirstColumn';
import DocumentService from '../services/DocumentService';

const drawerWidth = 240;
const closedDrawerWidth = 68;

const MainScreen: React.FC = () => {
    const [isDrawerOpen, setIsDrawerOpen] = useState(true);
    const [imageSrc, setImageSrc] = useState<string | null>(null);
    const [base64Text, setBase64Text] = useState<string | null>(null);
    const [documentType, setDocumentType] = useState<string>('');
    const [uploadedFile, setUploadedFile] = useState<File | null>(null);

    const toggleDrawer = () => {
        setIsDrawerOpen((prev) => !prev);
    };

    const onDrop = (acceptedFiles: File[]) => {
        const file = acceptedFiles[0];
        if (file) {
            setUploadedFile(file);
            const reader = new FileReader();
            reader.onload = () => {
                setImageSrc(reader.result as string);
                setBase64Text(reader.result?.toString().split(',')[1] || null);
            };
            reader.readAsDataURL(file);
        }
    };

    const handleDocumentTypeChange = (event: SelectChangeEvent<string>) => {
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

    return (
        <Box
            sx={{
                display: 'flex',
                height: '100vh', // Constrain to viewport height
                overflow: 'hidden', // Prevent scrolling on the browser window
            }}
        >
            {/* Button to toggle the drawer */}
            <IconButton
                onClick={toggleDrawer}
                sx={{
                    position: 'absolute',
                    top: 16,
                    left: 16,
                    zIndex: 1300, // Ensure it appears above the drawer
                }}
            >
                <MenuIcon />
            </IconButton>

            {/* Drawer on the left */}
            <Drawer
                variant="persistent"
                open={isDrawerOpen}
                sx={{
                    width: isDrawerOpen ? drawerWidth : closedDrawerWidth,
                    flexShrink: 0,
                    '& .MuiDrawer-paper': {
                        width: isDrawerOpen ? drawerWidth : closedDrawerWidth,
                        boxSizing: 'border-box',
                    },
                }}
            >
                <Toolbar />
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
                        Drop an image file here
                    </Typography>
                </Box>
            </Drawer>

            {/* Main content area */}
            <Box
                component="main"
                sx={{
                    flexGrow: 1,
                    display: 'grid',
                    gridTemplateColumns: '1fr 200px 1fr 100px 1fr',
                    gap: '16px',
                    height: '100vh', // Constrain to viewport height
                    overflow: 'hidden', // Prevent content overflow
                }}
            >
                {/* First column: Image and text */}
                <FirstColumn imageSrc={imageSrc} base64Text={base64Text} />

                {/* Second column: Identify Document and OCR */}
                <Box
                    sx={{
                        backgroundColor: '#d3d3d3',
                        display: 'flex',
                        flexDirection: 'column',
                        justifyContent: 'flex-start',
                        alignItems: 'center',
                        height: '100%',
                        padding: '16px',
                        gap: '16px',
                    }}
                >
                    {/* Identify Document Button */}
                    <Button variant="contained" color="primary" onClick={handleIdentifyDocument}>
                        Identify Document
                    </Button>

                    {/* Document Type Selector */}
                    <FormControl fullWidth>
                        <InputLabel id="document-type-label">Document Type</InputLabel>
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

                    {/* OCR Document Button */}
                    <Button
                        variant="contained"
                        color="secondary"
                        disabled={!documentType}
                    >
                        OCR Document
                    </Button>
                </Box>

                {/* Third column: Placeholder */}
                <Box
                    sx={{
                        backgroundColor: '#b0c4de',
                        display: 'flex',
                        justifyContent: 'center',
                        alignItems: 'center',
                        height: '100%',
                        overflow: 'auto', // Independent scrolling
                    }}
                >
                    <Typography>Column 3</Typography>
                </Box>

                {/* Fourth column: Placeholder */}
                <Box
                    sx={{
                        backgroundColor: '#d3d3d3',
                        display: 'flex',
                        justifyContent: 'center',
                        alignItems: 'center',
                        height: '100%',
                    }}
                >
                    <Typography>Column 4</Typography>
                </Box>

                {/* Fifth column: Placeholder */}
                <Box
                    sx={{
                        backgroundColor: '#b0c4de',
                        display: 'flex',
                        justifyContent: 'center',
                        alignItems: 'center',
                        height: '100%',
                        overflow: 'auto', // Independent scrolling
                    }}
                >
                    <Typography>Column 5</Typography>
                </Box>
            </Box>
        </Box>
    );
};

export default MainScreen;