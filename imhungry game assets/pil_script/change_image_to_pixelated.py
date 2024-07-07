from PIL import Image

# Load the image
image_path = "D:\\Users\\SenpaiOrigin\\Downloads\\pil script\\greek_salad.webp"
img = Image.open(image_path)

# Resize the image to make it smaller
smaller_img = img.resize((96, 96), Image.Resampling.NEAREST)

# Save the resized image
smaller_img_path = "D:\\Users\\SenpaiOrigin\\Downloads\\pil script\\greek_salad_96.png"
smaller_img.save(smaller_img_path)