# FontAwesomeToBunqAvatar
Small console application that converts a font-awesome icon to an avatar that can be used for bunq accounts.

# Usage
```
FontAwesomeToBunqAvatar     [-h] [--color COLOR] [--filename FILENAME]
                            [--font FONT] [--showIcons] [--size SIZE]
                            icon [icon ...]

Options:
      --font=VALUE           Font file to use (default: fontawesome-webfon-
                               t.ttf)
      --htmlcolor=VALUE      Color (HTML color code or name, default: black)
      --filename=VALUE       The name of the output file (it must end with -
                               ".png"). If all files are exported, it is used
                               as a prefix.
      --size=VALUE           Size of image (default: 420)
      --showIcons            Show available icons and exit
      -h, -?, --help             Displays this help text

Example:
FontAwesomeToBunqAvatar --htmlcolor=FFAA11 plane
```
