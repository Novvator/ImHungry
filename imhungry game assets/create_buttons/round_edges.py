from PIL import Image, ImageDraw, ImageFont
import os

def create_pixelated_button(text, color1, color2, border_color="#000000", border_width=5, width=300, height=100, font_size=80, font_filename="arial.ttf", vertical_offset=0):
    current_dir = os.getcwd()
    
    # Construct the font path and save path relative to the current working directory
    font_path = os.path.join(current_dir, font_filename)
    save_path = os.path.join(current_dir, f"{text}_button.png")
    
    # Calculate the dimensions with the border
    full_width = width + 2 * border_width
    full_height = height + 2 * border_width
    corner_radius = height // 2

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
    shadow_offset = 2
    draw.text((text_x + shadow_offset, text_y + shadow_offset), text, font=font, fill="gray")
    
    # Draw the text on the button
    draw.text((text_x, text_y), text, font=font, fill="black")
    
    # Save the image
    button_image.save(save_path, "PNG")
    
    print(f"Pixelated button '{text}' created and saved as {text}_pixelated_button.png")

# Example usage with a custom pixelated font
create_pixelated_button("Kosmos 2", "#faf20f", "#ffbb00", vertical_offset=0, font_filename="pixelated.ttf")
