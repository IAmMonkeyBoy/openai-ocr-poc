# OpenAI OCR POC

## Overview

The **OpenAI OCR POC** is a proof-of-concept application designed to demonstrate the use of Optical Character
Recognition (OCR) for processing various document types, such as invoices, fuel receipts, and more. 

## Features

- Upload and preview images of documents.
- Identify document types (e.g., Invoice, Fuel Receipt, etc.).
- Perform OCR to extract text and structured data.
- Display extracted data in a user-friendly format.
- Support for multiple document types with customizable models.

## Technologies Used

- **Frontend**: React with Material-UI for the user interface.
- **Backend**: C# with .NET 10 for processing and business logic.
- **OCR**: Integration with OCR libraries or APIs for text extraction.
- **Data Models**: Strongly-typed C# models for structured data representation.

## Usage

1. Drag and drop or upload a document image.  (samples may be found in the samples folder. These are just random images from the web of unknown origin or license. They should not be included in any product intended for commercial use.)
2. Select the document type from the dropdown menu or Click the Identify button to classify the document.
3. Click the OCR Document button to extract text and structured data.
4. View the extracted data in the results display area.
