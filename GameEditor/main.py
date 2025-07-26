import cv2

img=cv2.imread("bee.jpeg")
resized = cv2.resize(img, (512, 512))
gray = cv2.cvtColor(resized, cv2.COLOR_BGR2GRAY)
_, bw = cv2.threshold(gray, 128, 255, cv2.THRESH_BINARY_INV)

colored = cv2.cvtColor(bw, cv2.COLOR_GRAY2RGBA)
cv2.imwrite("bee_empty.png", colored)
