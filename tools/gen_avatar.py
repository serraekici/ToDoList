#!/usr/bin/env python3
"""Stardew-like chibi avatar layers at 48x64."""
from pathlib import Path
from PIL import Image

W, H = 48, 64
OUT = (28, 22, 18, 255)
S = (232, 168, 130, 255)
D = (196, 124, 92, 255)
L = (245, 201, 168, 255)
E = (0, 255, 255, 255)
WHITE = (255, 255, 255, 255)
BLUSH = (236, 150, 150, 255)
HH = (255, 0, 255, 255)
HA = (180, 0, 180, 255)
HI = (255, 120, 255, 255)
TRANS = (0, 0, 0, 0)

ROOT = Path("/Users/serra.ekici/Desktop/ToDoList/assets/avatar/layers")


def blank():
    return Image.new("RGBA", (W, H), TRANS)


def put(im, x, y, c):
    if 0 <= x < W and 0 <= y < H:
        im.putpixel((x, y), c)


def rect(im, x, y, w, h, c):
    for i in range(w):
        for j in range(h):
            put(im, x + i, y + j, c)


def ellipse(im, cx, cy, rx, ry, c):
    for y in range(cy - ry - 1, cy + ry + 2):
        for x in range(cx - rx - 1, cx + rx + 2):
            if ((x + 0.5 - cx) / rx) ** 2 + ((y + 0.5 - cy) / ry) ** 2 <= 1:
                put(im, x, y, c)


def outline(im, color=OUT):
    src = im.copy()
    for y in range(H):
        for x in range(W):
            if src.getpixel((x, y))[3]:
                continue
            for dx, dy in ((-1, 0), (1, 0), (0, -1), (0, 1)):
                nx, ny = x + dx, y + dy
                if 0 <= nx < W and 0 <= ny < H and src.getpixel((nx, ny))[3]:
                    im.putpixel((x, y), color)
                    break
    return im


def shade_bottom(im, fill, dark, y0):
    for y in range(y0, H):
        for x in range(W):
            p = im.getpixel((x, y))
            if p == fill:
                put(im, x, y, dark)


def punch_face(im, bangs_y=15):
    """Keep hair off the face so eyes stay visible."""
    for y in range(bangs_y + 1, 32):
        for x in range(12, 37):
            if ((x + 0.5 - 24) / 11.5) ** 2 + ((y + 0.5 - 21) / 9.5) ** 2 <= 1:
                put(im, x, y, TRANS)


def bangs(im, depth=4):
    rect(im, 14, 10, 20, depth, HH)
    rect(im, 16, 11, 7, 2, HI)
    for x in range(15, 34, 3):
        put(im, x, 10 + depth - 1, HA)


def hair_cap(im):
    """Skull cap only — does not cover the face."""
    ellipse(im, 24, 11, 14, 10, HH)
    ellipse(im, 23, 9, 8, 5, HI)
    rect(im, 10, 12, 4, 10, HH)
    rect(im, 34, 12, 4, 10, HH)
    rect(im, 10, 18, 4, 5, HA)
    rect(im, 34, 18, 4, 5, HA)


def body():
    im = blank()
    ellipse(im, 24, 18, 13, 14, S)
    rect(im, 20, 29, 8, 3, S)
    rect(im, 21, 32, 6, 4, S)
    rect(im, 17, 35, 14, 12, S)
    rect(im, 12, 36, 5, 14, S)
    rect(im, 31, 36, 5, 14, S)
    rect(im, 12, 49, 5, 3, S)
    rect(im, 31, 49, 5, 3, S)
    rect(im, 18, 46, 5, 13, S)
    rect(im, 25, 46, 5, 13, S)
    shade_bottom(im, S, D, 27)
    rect(im, 16, 9, 5, 3, L)
    rect(im, 15, 10, 3, 2, L)
    rect(im, 14, 24, 3, 2, BLUSH)
    rect(im, 31, 24, 3, 2, BLUSH)
    outline(im)

    def eye(x):
        rect(im, x, 17, 6, 5, OUT)
        rect(im, x + 1, 18, 4, 3, E)
        put(im, x + 2, 19, WHITE)
        put(im, x + 4, 20, (20, 16, 14, 255))
        rect(im, x, 16, 6, 1, OUT)
        put(im, x - 1, 17, OUT)
        put(im, x + 6, 17, OUT)

    eye(14)
    eye(28)
    put(im, 24, 24, D)
    rect(im, 23, 27, 3, 1, (120, 70, 70, 255))
    return im


def glasses():
    im = blank()
    frame = (22, 18, 18, 255)
    def frame_box(x, y, w, h):
        rect(im, x, y, w, 1, frame)
        rect(im, x, y + h - 1, w, 1, frame)
        rect(im, x, y, 1, h, frame)
        rect(im, x + w - 1, y, 1, h, frame)
    frame_box(13, 17, 8, 6)
    frame_box(27, 17, 8, 6)
    rect(im, 21, 19, 6, 1, frame)
    put(im, 12, 19, frame)
    put(im, 35, 19, frame)
    return im


def hair_short():
    im = blank()
    hair_cap(im)
    bangs(im, 3)
    punch_face(im, 13)
    outline(im)
    return im


def hair_part():
    im = blank()
    hair_cap(im)
    rect(im, 12, 9, 12, 5, HH)
    rect(im, 24, 10, 12, 4, HH)
    rect(im, 13, 10, 6, 2, HI)
    punch_face(im, 14)
    outline(im)
    return im


def hair_spike():
    im = blank()
    hair_cap(im)
    spikes = [
        (14, 2, 3, 7), (17, 0, 3, 9), (20, 1, 3, 8), (23, 0, 4, 10),
        (27, 1, 3, 8), (30, 0, 3, 9), (33, 3, 3, 6), (11, 5, 3, 6),
    ]
    for x, y, w, h in spikes:
        rect(im, x, y, w, h, HH)
        put(im, x + 1, y + 1, HI)
        put(im, x + w - 1, y + 2, HA)
    bangs(im, 3)
    punch_face(im, 13)
    outline(im)
    return im


def hair_long():
    im = blank()
    hair_cap(im)
    rect(im, 8, 16, 6, 24, HH)
    rect(im, 34, 16, 6, 24, HH)
    for y in (20, 26, 32, 38):
        rect(im, 7, y, 3, 4, HH)
        rect(im, 38, y, 3, 4, HH)
        rect(im, 8, y + 2, 4, 3, HA)
        rect(im, 36, y + 2, 4, 3, HA)
    bangs(im, 4)
    punch_face(im, 14)
    outline(im)
    return im


def hair_pony():
    im = blank()
    hair_cap(im)
    rect(im, 30, 10, 8, 6, HH)
    rect(im, 33, 14, 7, 20, HH)
    rect(im, 34, 18, 5, 18, HA)
    rect(im, 35, 12, 3, 4, HI)
    bangs(im, 4)
    punch_face(im, 14)
    outline(im)
    return im


def top_tee():
    im = blank()
    c, cd = (72, 76, 82, 255), (48, 50, 56, 255)
    rect(im, 16, 34, 16, 13, c)
    rect(im, 16, 43, 16, 4, cd)
    rect(im, 12, 35, 5, 6, c)
    rect(im, 31, 35, 5, 6, c)
    rect(im, 12, 39, 5, 2, cd)
    rect(im, 31, 39, 5, 2, cd)
    rect(im, 21, 34, 6, 2, TRANS)
    outline(im)
    return im


def top_hoodie():
    im = blank()
    c, cd = (126, 164, 132, 255), (86, 120, 94, 255)
    rect(im, 15, 33, 18, 15, c)
    rect(im, 15, 44, 18, 4, cd)
    rect(im, 11, 35, 5, 16, c)
    rect(im, 32, 35, 5, 16, c)
    rect(im, 11, 47, 5, 4, cd)
    rect(im, 32, 47, 5, 4, cd)
    rect(im, 16, 31, 16, 4, cd)
    rect(im, 20, 43, 8, 2, (240, 236, 210, 255))
    outline(im)
    return im


def top_vest():
    im = blank()
    c, cd = (176, 72, 72, 255), (128, 48, 48, 255)
    inner = (245, 232, 200, 255)
    rect(im, 17, 34, 14, 13, inner)
    rect(im, 16, 34, 5, 13, c)
    rect(im, 27, 34, 5, 13, c)
    rect(im, 16, 43, 16, 4, cd)
    outline(im)
    return im


def top_tank():
    im = blank()
    c, cd = (232, 130, 160, 255), (196, 90, 120, 255)
    rect(im, 17, 35, 14, 12, c)
    rect(im, 17, 43, 14, 4, cd)
    rect(im, 18, 32, 3, 4, c)
    rect(im, 27, 32, 3, 4, c)
    outline(im)
    return im


def top_polo():
    im = blank()
    c, cd = (244, 236, 214, 255), (210, 196, 168, 255)
    col = (90, 110, 80, 255)
    rect(im, 16, 34, 16, 13, c)
    rect(im, 16, 43, 16, 4, cd)
    rect(im, 12, 35, 5, 6, c)
    rect(im, 31, 35, 5, 6, c)
    rect(im, 20, 33, 4, 3, col)
    rect(im, 24, 33, 4, 3, col)
    put(im, 24, 37, col)
    outline(im)
    return im


def bottom_pants():
    im = blank()
    c, cd = (92, 72, 56, 255), (62, 46, 34, 255)
    rect(im, 17, 46, 6, 13, c)
    rect(im, 25, 46, 6, 13, c)
    rect(im, 17, 55, 6, 4, cd)
    rect(im, 25, 55, 6, 4, cd)
    rect(im, 16, 59, 8, 4, (58, 42, 32, 255))
    rect(im, 24, 59, 8, 4, (58, 42, 32, 255))
    outline(im)
    return im


def bottom_grey():
    im = blank()
    c, cd = (164, 162, 156, 255), (120, 118, 114, 255)
    rect(im, 17, 46, 6, 13, c)
    rect(im, 25, 46, 6, 13, c)
    rect(im, 17, 55, 6, 4, cd)
    rect(im, 25, 55, 6, 4, cd)
    rect(im, 16, 59, 8, 4, (90, 90, 94, 255))
    rect(im, 24, 59, 8, 4, (90, 90, 94, 255))
    outline(im)
    return im


def bottom_shorts():
    im = blank()
    c, cd = (196, 168, 110, 255), (160, 130, 80, 255)
    rect(im, 17, 46, 6, 8, c)
    rect(im, 25, 46, 6, 8, c)
    rect(im, 17, 51, 6, 3, cd)
    rect(im, 25, 51, 6, 3, cd)
    rect(im, 16, 59, 8, 4, (186, 186, 190, 255))
    rect(im, 24, 59, 8, 4, (186, 186, 190, 255))
    outline(im)
    return im


def bottom_skirt():
    im = blank()
    c, cd = (240, 210, 110, 255), (210, 170, 70, 255)
    rect(im, 16, 46, 16, 9, c)
    for x in range(17, 32, 3):
        rect(im, x, 47, 1, 7, cd)
    rect(im, 16, 52, 16, 3, cd)
    rect(im, 16, 59, 8, 4, (232, 130, 160, 255))
    rect(im, 24, 59, 8, 4, (232, 130, 160, 255))
    outline(im)
    return im


LAYERS = {
    "body": body,
    "acc-glasses": glasses,
    "hair-short": hair_short,
    "hair-part": hair_part,
    "hair-spike": hair_spike,
    "hair-long": hair_long,
    "hair-pony": hair_pony,
    "top-tee": top_tee,
    "top-hoodie": top_hoodie,
    "top-vest": top_vest,
    "top-tank": top_tank,
    "top-polo": top_polo,
    "bottom-pants": bottom_pants,
    "bottom-grey": bottom_grey,
    "bottom-shorts": bottom_shorts,
    "bottom-skirt": bottom_skirt,
}


def recolor(im, mapping):
    out = im.copy()
    px = out.load()
    for y in range(H):
        for x in range(W):
            r, g, b, a = px[x, y]
            if not a:
                continue
            key = (r, g, b)
            if key in mapping:
                nr, ng, nb = mapping[key]
                px[x, y] = (nr, ng, nb, a)
    return out


def compose(hair, top, bottom, glasses_on, skin=None, hair_col=None, eye=None):
    skin = skin or {"S": (232, 168, 130), "D": (196, 124, 92), "L": (245, 201, 168)}
    hair_col = hair_col or {"H": (107, 62, 38), "A": (74, 40, 24), "I": (148, 96, 56)}
    eye = eye or (92, 58, 32)
    body_map = {
        S[:3]: skin["S"],
        D[:3]: skin["D"],
        L[:3]: skin["L"],
        E[:3]: eye,
    }
    hair_map = {
        HH[:3]: hair_col["H"],
        HA[:3]: hair_col["A"],
        HI[:3]: hair_col["I"],
    }
    canvas = blank()
    canvas.alpha_composite(recolor(LAYERS["body"](), body_map))
    canvas.alpha_composite(LAYERS["bottom-" + bottom]())
    canvas.alpha_composite(LAYERS["top-" + top]())
    canvas.alpha_composite(recolor(LAYERS["hair-" + hair](), hair_map))
    if glasses_on:
        canvas.alpha_composite(LAYERS["acc-glasses"]())
    return canvas


def main():
    ROOT.mkdir(parents=True, exist_ok=True)
    for name, fn in LAYERS.items():
        im = fn()
        im.save(ROOT / f"{name}.png")
        print("wrote", name, im.size)
    girl = compose("long", "tank", "skirt", False)
    boy = compose("spike", "tee", "shorts", True)
    girl.save(ROOT / "_preview-girl.png")
    boy.save(ROOT / "_preview-boy.png")
    # upscale previews for inspection
    girl.resize((192, 256), Image.NEAREST).save(ROOT / "_preview-girl-x4.png")
    boy.resize((192, 256), Image.NEAREST).save(ROOT / "_preview-boy-x4.png")


if __name__ == "__main__":
    main()
