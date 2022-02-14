# Cube-City-Unity-Sprint
C# Unity3D - Cube City

![cubecity2](https://user-images.githubusercontent.com/5803874/153745292-6f7a4cc6-1ad0-4515-a969-dd5199788f04.jpg)
![cubecity3](https://user-images.githubusercontent.com/5803874/153745295-cf845d52-ca0b-4f7f-98ab-529a4533470f.jpg)
![cubecity4](https://user-images.githubusercontent.com/5803874/153745299-f10ac4f5-e3ea-4cf4-adc4-ee6ab65dbe4b.jpg)
![cubecity1](https://user-images.githubusercontent.com/5803874/153745285-07bcadcf-bfab-40cf-97dd-c91db66cc058.jpg)

Project Goal: Procedurally generate Cube City with manhattan style skyscrapers, roads, cars, stoplights, and helicopters of "ANY SIZE"!

Uses MVC pattern (with the Models not using UnityEngine), some command pattern, observer pattern, and recursive elements. Just one object with one script (CubeCity.cs) needed in scene. The Developer can dynamically set city attributes such as number of blocks in X, Z. Size of city blocks in X, Z. Avg number of Helicopters per block, Avg number of Super Skyscrapers per block, etc. Helicopter paths are checked to see if they fly through buildings. Cars stop at stoplights with waiting coroutines getting delay info from stoplight, no OnUpdate functions. Switch between 3 different camera views to see the city, follow helicopters, or follow cars. 
