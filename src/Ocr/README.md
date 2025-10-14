# OCR library based on Tesseract

## Prerequisites
Make sure you have the following prerequisites installed:
- [Tesseract](https://github.com/charlesw/tesseract)
    - For Linux
        - Execute `sudo apt update && apt-get -q -y install libleptonica-dev libtesseract-dev` to install the required packages
    - For macOS
        - Make sure you have [Homebrew](https://brew.sh) installed
        - Execute `brew install leptonica tesseract` to install the required packages

Make sure your application has `./tessdata` folder with the required language files.
