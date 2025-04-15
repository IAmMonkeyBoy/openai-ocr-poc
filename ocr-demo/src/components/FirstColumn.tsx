import React from 'react';
import { Box } from '@mui/material';

interface FirstColumnProps {
    imageSrc: string | null;
}

const FirstColumn: React.FC<FirstColumnProps> = ({ imageSrc }) => {
    return (
        <Box
sx={{
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'flex-start',
                    height: '100%',
                    overflow: 'auto', // Independent scrolling
                    boxSizing: 'border-box', // Ensure padding and border are included in dimensions
                    width: '100%', // Take full width of the container
                }}
        >
            {/* Image container */}
            <Box
                sx={{
                    width: '400px', // Full width of the column
height: '516px',
                    overflow: 'hidden',
                    border: '1px solid #ccc',
                    borderRadius: '8px',
                    backgroundColor: '#fff',
                    marginBottom: '16px',
                    display: 'flex',
                    justifyContent: 'center',
                    alignItems: 'center',
                    flexGrow: 1,
                }}
            >
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
            </Box>


        </Box>
    );
};

export default FirstColumn;