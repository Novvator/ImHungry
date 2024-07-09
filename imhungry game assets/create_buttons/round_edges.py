from PIL import Image, ImageDraw, ImageFont
import os

def darken_color(hex_color, factor=0.7):
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

def create_pixelated_button(text, color1, color2, border_color="#000000", border_width=5, width=300, height=100, font_size=80, font_filename="arial.ttf", vertical_offset=0):
    current_dir = os.getcwd()
    
    # Construct the font path and save path relative to the current working directory
    font_path = os.path.join(current_dir, font_filename)
    save_path = os.path.join(current_dir, f"{text}_button.png")
    save_path_pressed = os.path.join(current_dir, f"{text}_button_pressed.png")
    
    # Calculate the dimensions with the border
    full_width = width + 2 * border_width
    full_height = height + 2 * border_width
    corner_radius = height // 2

    def create_button_image(color1, color2, save_path):
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

        # Create the top half and bottom half images
        top_half = Image.new("RGBA", (width, height // 2), color1)
        bottom_half = Image.new("RGBA", (width, height // 2), color2)

        # Combine top and bottom halves
        half_button = Image.new("RGBA", (width, height))
        half_button.paste(top_half, (0, 0))
        half_button.paste(bottom_half, (0, height // 2))

        # Apply the mask to the combined image
        button_image.paste(half_button, (border_width, border_width), mask=mask)

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
    create_button_image(color1, color2, save_path)
    
    # Create pressed button with darkened colors
    darkened_color1 = darken_color(color1, factor=0.5)
    darkened_color2 = darken_color(color2, factor=0.5)
    create_button_image(darkened_color1, darkened_color2, save_path_pressed)
    
    print(f"Pixelated button '{text}' created and saved as {text}_button.png and {text}_button_pressed.png")

# Example usage with a custom pixelated font
create_pixelated_button("TRAILS", "#faf20f", "#ffbb00", vertical_offset=0, font_filename="pixelated.ttf", width=1100, height=378,font_size=330,border_width=20)

# Example usage with a custom pixelated font
# create_pixelated_button("WORLD 1", "#faf20f", "#ffbb00", vertical_offset=0, font_filename="pixelated.ttf", width=1100, height=378,font_size=330,border_width=20)
# create_pixelated_button("WORLD 2", "#fc835e", "#e2ffff", vertical_offset=0, font_filename="pixelated.ttf", width=1100, height=378,font_size=330,border_width=20)
# create_pixelated_button("WORLD 3", "#0067ca", "#ffffff", vertical_offset=0, font_filename="pixelated.ttf", width=1100, height=378,font_size=330,border_width=20)

# create_pixelated_button("DEFAULT", "#8f8f8f", "#cbcbcb", vertical_offset=0, font_filename="pixelated.ttf", width=1200, height=378,font_size=330,border_width=20)