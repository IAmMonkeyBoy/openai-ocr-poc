import React from 'react';
                import { Box, Typography, Table, TableBody, TableCell, TableHead, TableRow } from '@mui/material';
                import { QualityAssessment } from '../services/DocumentService';
                
                interface QualityAssessmentDisplayProps {
                    qualityAssessment: QualityAssessment | null;
                }
                
                const QualityAssessmentDisplay: React.FC<QualityAssessmentDisplayProps> = ({ qualityAssessment }) => {
                    if (!qualityAssessment || !qualityAssessment.FieldEvaluations || qualityAssessment.FieldEvaluations.length === 0) {
                        return (
                            <Box sx={{ padding: 2 }}>
                                <Typography>No Quality Assessment data available.</Typography>
                            </Box>
                        );
                    }
                
                    return (
                        <Box sx={{ padding: 2 }}>
                            <Typography variant="h6" gutterBottom>
                                Quality Assessment
                            </Typography>
                            <Table>
                                <TableHead>
                                    <TableRow>
                                        <TableCell>Field Name</TableCell>
                                        <TableCell>Is Present</TableCell>
                                        <TableCell>Priority</TableCell>
                                        <TableCell>Value</TableCell>
                                    </TableRow>
                                </TableHead>
                                <TableBody>
                                    {qualityAssessment.FieldEvaluations.map((field, index) => (
                                        <TableRow
                                            key={index}
                                            sx={{
                                                backgroundColor: !field.IsPresent && (field.Priority === 'High' || field.Priority === 'Medium') 
                                                    ? 'rgba(255, 0, 0, 0.1)' // Pale red
                                                    : 'inherit',
                                            }}
                                        >
                                            <TableCell>{field.FieldName}</TableCell>
                                            <TableCell>{field.IsPresent ? 'Yes' : 'No'}</TableCell>
                                            <TableCell>{field.Priority}</TableCell>
                                            <TableCell>{field.Value}</TableCell>
                                        </TableRow>
                                    ))}
                                </TableBody>
                            </Table>
                        </Box>
                    );
                };
                
                export default QualityAssessmentDisplay;