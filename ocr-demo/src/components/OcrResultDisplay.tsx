import React, {JSX} from 'react';
import RetroEncabulatorProgress from './RetroEncabulatorProgress';
import {OcrDocumentResponse} from '../services/DocumentService';
import {Box, Typography} from '@mui/material';

// Helper function to convert camel case to title case with spaces
const camelCaseToTitleCase = (str: string): string => {
    return str
        .replace(/([a-z])([A-Z])/g, '$1 $2') // Add space between camel case words
        .replace(/^./, (char) => char.toUpperCase()); // Capitalize the first letter
};

const renderJson = (data: object, level: number = 0): JSX.Element => {
    if (typeof data !== 'object' || data === null) {
        // Skip rendering for null values
        if (data === null) return <></>;

        // Render primitive values inline with the label
        return (
            <Typography sx={{ marginLeft: level * 2, fontSize: '0.875rem', display: 'inline' }}>
                {String(data)}
            </Typography>
        );
    }

    if (Array.isArray(data)) {
        // Render arrays with a subtle visual indicator (e.g., bullet points)
        return (
            <Box sx={{ marginLeft: level * 2, marginBottom: 1 }}>
                {data.map((item, index) => (
                    <Box key={index} sx={{ marginLeft: 2, display: 'flex', alignItems: 'center' }}>
                        <Typography sx={{ fontSize: '0.875rem', marginRight: 1 }}>•</Typography>
                        {renderJson(item, level + 1)}
                    </Box>
                ))}
            </Box>
        );
    }

    // Render objects
    return (
        <Box sx={{ marginLeft: level * 2, marginBottom: 1 }}>
            {Object.entries(data).map(([key, value]) => (
                <Box key={key} sx={{ marginBottom: 0.5 }}>
                    <Typography sx={{ fontSize: '0.875rem', fontWeight: 'bold', display: 'inline' }}>
                        {camelCaseToTitleCase(key)}:
                    </Typography>{' '}
                    {typeof value !== 'object' ? (
                        value !== null && (
                            <Typography sx={{ fontSize: '0.875rem', display: 'inline' }}>
                                {String(value)}
                            </Typography>
                        )
                    ) : (
                        renderJson(value, level + 1)
                    )}
                </Box>
            ))}
        </Box>
    );
};
interface OcrResultDisplayProps {
    loading: boolean;
    ocrResult: OcrDocumentResponse | null;
    documentType: string | null; 
}

const OcrResultDisplay: React.FC<OcrResultDisplayProps> = ({ loading, ocrResult, documentType }) => {
    if (loading) {
        return (
            <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', height: '100%' }}>
                <RetroEncabulatorProgress />
            </Box>
        );
    }

    if (ocrResult) {
        let documentData;
        try {
            // Parse only if it's a string
            documentData = typeof ocrResult.Document === 'string'
                ? JSON.parse(ocrResult.Document)
                : ocrResult.Document;
        } catch (error) {
            console.error('Failed to parse document:', error);
            return (
                <Box sx={{ padding: 2 }}>
                    <Typography color="error">Invalid document format.</Typography>
                </Box>
            );
        }

        return (
            <Box sx={{ padding: 2 }}>
                {documentType && (
                    <Typography variant="h6" gutterBottom sx={{  fontWeight: 'bold' }}>
                        {documentType}
                    </Typography>
                )}
                {renderJson(documentData)}
            </Box>
        );
    }

    return (
        <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', height: '100%' }}>
            <Typography>Upload, Identify and OCR a document to see the result here.</Typography>
        </Box>
    );
};

export default OcrResultDisplay;