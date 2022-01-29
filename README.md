# Windows Programming CQ2019/3 - HCMUS
# Project PAINT

## Notice:
- The "shapes" folder in the execution directory contains all shapes for the application to run. 
- If you want to add a shape, simply copy its .dll file and paste it there. Similarly, if you want to remove a shape, just delete its .dll file inside the folder.
- The "shapes" folder can be empty/absent, though that way, the application is unable to draw anything.

## Team members:
- 19120268 - Ngô Đặng Gia Lâm
- 19120383 - Huỳnh Tấn Thọ
- 19120496 - Trần Thị Khánh Duyên

## Technical details
- Design Pattern: singleton, factory, object pool, prototype

## Core features
1. Dynamically load all graphic objects that can be drawn from external DLL files.
2. The user can choose which object to draw.
3. The user can see the preview of the object they want to draw.
4. The user can finish the drawing preview and their change becomes permanent with previously drawn objects.
5. The list of drawn objects can be saved and loaded again for continuing later.
6. Save and load all drawn objects as an image in bmp/png/jpg format (rasterization).

## Basic graphic objects
1. Line: controlled by two points, the starting point, and the endpoint.
2. Rectangle: controlled by two points, the left top point, and the right bottom point.
3. Ellipse: controlled by two points, the left top point, and the right bottom point.
4. Square: controlled by two points, the left top point, and the right bottom point.
5. Circle: controlled by two points, the left top point, and the right bottom point.

## Improvements
1. Two bonus shapes for the user to create: square and circle.
2. Allow the user to change the color, pen width, and stroke type (dash, dot, dash dot,...)
3. Adding image to the canvas.
4. Undo and Redo.
5. "New File" button to quickly start over, the application will of course ask the user for confirmation if any new changes is detected.
6. Objects in drawable list have their own icons instead of plain text.
7. Used Fluent.Ribbon to obtain MS Paint-like user interface.
8. Select a single shape to move around, rotate, resize, and delete.
9. Select a single shape to copy and paste.
10. Bulk interaction: hold control key to select multiple shapes to move, copy, paste, or delete.
