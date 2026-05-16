# HTML File Modifier

This program allows you to modify and edit multiple HTML files within a folder. It uses the **HtmlAgilityPack** library to parse and manipulate the HTML content.


## Features

- Processes and modifies multiple HTML files in a specified folder.
- CSS styling modifications for better presentation (center-aligned text, bold, etc.).
- Removes unwanted tags and content (e.g., `<a>`, footnotes).
- Replaces list tags (`<li>`) with paragraph tags (`<p>`).

## Requirements

To run this program, you will need:
- .NET Framework or .NET Core
- [HtmlAgilityPack](https://www.nuget.org/packages/HtmlAgilityPack/) library for parsing HTML

## How to Use

1. **Download** or **Clone** this repository.
2. Ensure that the **HtmlAgilityPack** library is installed via NuGet.
3. Run the program.
4. The program will prompt you to enter the path of the folder containing the HTML files.
5. Once the folder is selected, it will iterate through all `.html` files and apply the defined modifications.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contribution

Feel free to open issues or pull requests if you encounter any bugs or want to contribute improvements.
