import React, { useState, useEffect } from 'react';
import { Box, Typography, LinearProgress } from '@mui/material';

const RetroEncabulatorProgress: React.FC = () => {
    const steps = [
        "Parsing shipping logs for ephemeral cargo-linguistic embeddings",
        "Initializing forklift synergy with advanced semantic models",
        "Vectorizing large language manifests for neural forklift scheduling",
        "Orchestrating quantum container-laden natural language workflows",
        "Infusing NLP-based pallet orchestration with hyperdimensional parsing",
        "Auto-checking cargo compliance via GPT-driven logistics correlation",
        "Synthesizing maritime Bill of Lading data with AI-driven synergy",
        "Finalizing ephemeral docking instructions for forklift-linguistic entanglement",
        "Completing advanced doc-processing ephemeralities for shipping AI"
    ];

    const [currentStepIndex, setCurrentStepIndex] = useState(0);
    const [fade, setFade] = useState(true);

    useEffect(() => {
        const interval = setInterval(() => {
            setFade(false);
            setTimeout(() => {
                setCurrentStepIndex((prevIndex) => (prevIndex + 1) % steps.length);
                setFade(true);
            }, 300); // Match the fade-out duration
        }, 1500);

        return () => clearInterval(interval);
    }, [steps.length]);

    const currentStep = steps[currentStepIndex];

    return (
        <Box
            sx={{
                maxWidth: 600,
                margin: '0 auto',
                textAlign: 'center',
                padding: 2,
            }}
        >
            <Typography
                variant="body2"
                sx={{
                    color: 'gray',
                    transition: 'opacity 0.3s ease-in-out',
                    opacity: fade ? 1 : 0,
                }}
                gutterBottom
            >
                {currentStep}
            </Typography>
            <LinearProgress
                variant="indeterminate"
                sx={{ height: 10, borderRadius: 5, marginY: 2 }}
            />
        </Box>
    );
};

export default RetroEncabulatorProgress;