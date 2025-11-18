from PIL import Image
import os

curr_path = os.getcwd()

image_name = "popel.jpg"
# Load the image
image_path = os.path.join(curr_path, image_name)
img = Image.open(image_path)

# Resize the image to make it smaller
smaller_img = img.resize((270, 405), Image.Resampling.NEAREST)

# Save the resized image
smaller_img_name = os.path.splitext(image_name)[0] + "_pixeled.png"
smaller_img_path = os.path.join(curr_path, smaller_img_name)
print(smaller_img_path)
smaller_img.save(smaller_img_path)
