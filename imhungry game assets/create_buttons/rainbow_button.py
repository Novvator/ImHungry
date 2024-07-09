from PIL import Image, ImageDraw, ImageFont
import os

def darken_color(hex_color, factor=0.3):
    """
    Darkens a given hexadecimal color.

    Args:
    hex_color (str): The hex color string (e.g., '#fc835e').
    factor (float): The factor by which to darken the color (0 < factor < 1).

    Returns:
    str: The darkened hex color string (e.g., '#701c00').
    """
    # Ensure the hex color string starts with '#'
    if hex_color.startswith('#'):
        hex_color = hex_color[1:]

    # Convert the hex color to RGB components
    r = int(hex_color[0:2], 16)
    g = int(hex_color[2:4], 16)
    b = int(hex_color[4:6], 16)

    # Darken each component by the factor
    r = int(r * factor)
    g = int(g * factor)
    b = int(b * factor)

    # Convert the darkened RGB components back to a hex color string
    darkened_hex_color = f'#{r:02x}{g:02x}{b:02x}'
    print(f"darkened color {darkened_hex_color}")
    return darkened_hex_color

def lighten_color(hex_color, factor=0.4):
    """
    Lightens a given hexadecimal color.

    Args:
    hex_color (str): The hex color string (e.g., '#fc835e').
    factor (float): The factor by which to lighten the color (0 < factor < 1).

    Returns:
    str: The lightened hex color string (e.g., '#ffe5d5').
    """
    # Ensure the hex color string starts with '#'
    if hex_color.startswith('#'):
        hex_color = hex_color[1:]

    # Convert the hex color to RGB components
    r = int(hex_color[0:2], 16)
    g = int(hex_color[2:4], 16)
    b = int(hex_color[4:6], 16)

    # Lighten each component by the factor
    r = int(r + (255 - r) * factor)
    g = int(g + (255 - g) * factor)
    b = int(b + (255 - b) * factor)

    # Convert the lightened RGB components back to a hex color string
    lightened_hex_color = f'#{r:02x}{g:02x}{b:02x}'
    print(f"lightened color {lightened_hex_color}")
    return lightened_hex_color

def create_pixelated_button(text, colors, border_color="#000000", border_width=5, width=300, height=100, font_size=80, font_filename="arial.ttf", vertical_offset=0):
    current_dir = os.getcwd()
    
    # Construct the font path and save path relative to the current working directory
    font_path = os.path.join(current_dir, font_filename)
    save_path = os.path.join(current_dir, f"{text}_button.png")
    save_path_pressed = os.path.join(current_dir, f"{text}_button_pressed.png")
    
    # Calculate the dimensions with the border
    full_width = width + 2 * border_width
    full_height = height + 2 * border_width
    corner_radius = height // 2

    def create_button_image(colors, save_path):
        # Create an image with the given size including the border
        button_image = Image.new("RGBA", (full_width, full_height), (255, 255, 255, 0))  # Using RGBA for transparency
        
        # Create a draw object
        draw = ImageDraw.Draw(button_image)

        # Draw the border
        draw.rounded_rectangle([(0, 0), (full_width, full_height)], radius=corner_radius, fill=border_color)

        # Create the mask for the rounded rectangle inside the border
        mask = Image.new("L", (width, height), 0)
        mask_draw = ImageDraw.Draw(mask)
        mask_draw.rounded_rectangle([(0, 0), (width, height)], radius=corner_radius - border_width, fill=255)

        # Draw each color stripe
        stripe_height = height // len(colors)
        for i, color in enumerate(colors):
            stripe = Image.new("RGBA", (width, stripe_height), color)
            button_image.paste(stripe, (border_width, border_width + i * stripe_height), mask=mask.crop((0, i * stripe_height, width, (i + 1) * stripe_height)))

        # Load the custom pixelated font
        font = ImageFont.truetype(font_path, font_size)
        
        # Calculate text size and position
        text_bbox = font.getbbox(text)
        text_width = text_bbox[2] - text_bbox[0]
        text_height = text_bbox[3] - text_bbox[1]
        text_x = (full_width - text_width) // 2
        text_y = (full_height - text_height) // 2 - vertical_offset

        # Draw the shadow for the text
        shadow_offset = 7
        draw.text((text_x + shadow_offset, text_y + shadow_offset), text, font=font, fill="gray")
        
        # Draw the text on the button
        draw.text((text_x, text_y), text, font=font, fill="black")
        
        # Save the image
        button_image.save(save_path, "PNG")
    
    # Create normal button
    create_button_image(colors, save_path)
    
    # Create pressed button with darkened colors
    darkened_colors = [darken_color(color, factor=0.5) for color in colors]
    create_button_image(darkened_colors, save_path_pressed)
    
    print(f"Pixelated button '{text}' created and saved as {text}_button.png and {text}_button_pressed.png")

# Rainbow colors
rainbow_colors = ["#ff0000", "#ff7f00", "#ffff00", "#00ff00", "#0000ff", "#4b0082", "#9400d3"]

# Example usage with a custom pixelated font
# create_pixelated_button("RAINBOW", rainbow_colors, vertical_offset=0, font_filename="pixelated.ttf", width=1200, height=378, font_size=330, border_width=20)

# Lightened rainbow colors for another button
lightened_rainbow_colors = [lighten_color(color) for color in rainbow_colors]
create_pixelated_button("DEFAULT", lightened_rainbow_colors, vertical_offset=0, font_filename="pixelated.ttf", width=1200, height=378, font_size=330, border_width=20)
