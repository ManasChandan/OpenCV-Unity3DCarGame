import cv2
import numpy as np
import socket
from pynput.keyboard import Key,Controller

def nothing(x):
    pass
s = socket.socket(socket.AF_INET , socket.SOCK_STREAM)
s.bind(('localhost',8888))
s.listen(5)

windowSize = 180

keyboard = Controller()

cap = cv2.VideoCapture(0)
kernel = np.ones((3,3),np.uint8)
font = cv2.FONT_HERSHEY_COMPLEX
cap.set(cv2.CAP_PROP_FRAME_WIDTH, 725)
cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 680)


while True:
    ret,frame = cap.read()
    if not ret:
        break
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break
    frame = cv2.GaussianBlur(frame,(11,11),0)
    frame2 = cv2.cvtColor(frame,cv2.COLOR_BGR2HSV)
    cv2.line(frame,(0,250),(680,250),(255,255,255),1)
    cv2.line(frame,(144,0),(144,680),(0,0,255),1)
    cv2.line(frame, (288, 0), (288, 680), (0, 0, 255), 1)
    cv2.line(frame, (432, 0), (432, 680), (0, 0, 255), 1)
    cv2.line(frame, (576, 0), (576, 680), (0, 0, 255), 1)
    lb = np.array([0,123,116])
    ub = np.array([255,255,255])
    mask = cv2.inRange(frame2,lb,ub)
    mask = cv2.erode(mask,None,iterations=2)
    mask = cv2.dilate(mask,None,iterations=2)
    opening = cv2.morphologyEx(mask,cv2.MORPH_OPEN,kernel)
    res = cv2.bitwise_and(frame,frame,mask=mask)

    contours,hierarchy = cv2.findContours(opening,cv2.RETR_TREE,cv2.CHAIN_APPROX_NONE)

    if len(contours)!=0:
        cnt = max(contours , key=cv2.contourArea)
        ((x,y) , radius) = cv2.minEnclosingCircle(cnt)
        M = cv2.moments(cnt)
        Cx = int(M['m10']/M['m00'])
        Cy = int(M['m01']/M['m00'])
        Centre = (Cx,Cy)
        cv2.circle(frame,(int(x),(int(y))),int(radius),(255,255,255),2)
        S = 'Location of object' + "(" +str(Cx) + "," + str(Cy) +")"
        cv2.putText(frame,S,(5,50),1,1,(0,0,255),1,lineType=cv2.LINE_AA)
        cv2.drawContours(frame,cnt,-1,(0,255,0),3)
        if(Centre[1] < 250):
            keyboard.press('w')
            keyboard.release('w')
        clientsocket, adress = s.accept()
        clientsocket.send(bytes(str(Cx), "utf-8"))
        clientsocket.close()



    cv2.imshow('original', frame)


cap.release()
cv2.destroyAllWindows()