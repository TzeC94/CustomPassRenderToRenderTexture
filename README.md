Unity version: 6.0

Sample of how to use Untiy HDRP Custom Pass to render opaque object with Alpha Test/Alpha Clipping into render texture without getting the alpha value write into the render texture

The solution is to add an override in shader to always output alpha of 1
![image](https://github.com/user-attachments/assets/81a3b130-01d5-49f4-a11e-4b50ca0672af)

The override will done through the command buffer before render and reset it after render
![image](https://github.com/user-attachments/assets/f385b00b-fffc-4d93-a970-182c427d6bdb)

Result with override
![image](https://github.com/user-attachments/assets/eedf2b87-5240-4016-b5ae-c671251b5689)

Result without override
![image](https://github.com/user-attachments/assets/4052fead-0b87-42b6-af0b-958555c4e476)
