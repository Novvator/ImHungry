from PIL import Image, ImageDraw
import random
import math

# -------------------------------------------
# CONFIGURATION
# -------------------------------------------
IMG_SIZE = 512

MARGIN = 80  # keeps the line away from edges  
STROKE_WIDTH = 80
BORDER_WIDTH = 12

STROKE_COLOR = (0, 200, 255)   # ciel / cyan
BORDER_COLOR = (0, 0, 0)

WIGGLE_AMOUNT = 0.15  # smooth randomness


def bezier_segment(start, end, wiggle_strength):
    """Cubic Bézier with two control points for bigger variation."""
    (x1, y1), (x2, y2) = start, end
    mx, my = (x1 + x2) / 2, (y1 + y2) / 2

    cx1 = mx + (random.random() - 0.5) * IMG_SIZE * wiggle_strength
    cy1 = my + (random.random() - 0.5) * IMG_SIZE * wiggle_strength

    cx2 = mx + (random.random() - 0.5) * IMG_SIZE * wiggle_strength
    cy2 = my + (random.random() - 0.5) * IMG_SIZE * wiggle_strength

    pts = []
    for t in [i / 70 for i in range(71)]:
        x = (1 - t)**3 * x1 + 3 * (1 - t)**2 * t * cx1 + 3 * (1 - t) * t**2 * cx2 + t**3 * x2
        y = (1 - t)**3 * y1 + 3 * (1 - t)**2 * t * cy1 + 3 * (1 - t) * t**2 * cy2 + t**3 * y2
        pts.append((int(x), int(y)))

    return pts


def generate_path():
    """Random path touching all 4 corners and center."""
    TL = (MARGIN, MARGIN)
    TR = (IMG_SIZE - MARGIN, MARGIN)
    BR = (IMG_SIZE - MARGIN, IMG_SIZE - MARGIN)
    BL = (MARGIN, IMG_SIZE - MARGIN)
    CENTER = (IMG_SIZE // 2, IMG_SIZE // 2)

    corners = [TL, TR, BR, BL]
    random.shuffle(corners)

    sequence = [corners[0], corners[1], CENTER, corners[2], corners[3], corners[0]]

    pts = []

    # Add main path
    for i in range(len(sequence) - 1):
        pts += bezier_segment(sequence[i], sequence[i + 1], WIGGLE_AMOUNT)

    return pts, sequence[0], sequence[-1]  # return start and end points for rounded caps


def draw_path(points, start_point, end_point, filename="pattern.png"):
    img = Image.new("RGBA", (IMG_SIZE, IMG_SIZE), (255, 255, 255, 0))
    draw = ImageDraw.Draw(img)

    # Draw border
    draw.line(points, fill=BORDER_COLOR, width=STROKE_WIDTH + BORDER_WIDTH*2, joint="curve")
    # Draw stroke
    draw.line(points, fill=STROKE_COLOR, width=STROKE_WIDTH, joint="curve")

    # Draw rounded start/end caps (radius = half the stroke width)
    r = STROKE_WIDTH // 2
    # start cap
    draw.ellipse([start_point[0]-r, start_point[1]-r, start_point[0]+r, start_point[1]+r],
                 fill=STROKE_COLOR, outline=BORDER_COLOR, width=BORDER_WIDTH)
    # end cap
    draw.ellipse([end_point[0]-r, end_point[1]-r, end_point[0]+r, end_point[1]+r],
                 fill=STROKE_COLOR, outline=BORDER_COLOR, width=BORDER_WIDTH)

    img.save(filename)
    print("Saved:", filename)


def generate_patterns(n=5):
    for i in range(n):
        pts, start, end = generate_path()
        draw_path(pts, start, end, f"pattern_{i+1}.png")


if __name__ == "__main__":
    generate_patterns(5)
