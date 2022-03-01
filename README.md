# Cube-City-Unity-Sprint
C# Unity3D - Cube City

![cubecity2](https://user-images.githubusercontent.com/5803874/153745292-6f7a4cc6-1ad0-4515-a969-dd5199788f04.jpg)
![cubecity3](https://user-images.githubusercontent.com/5803874/153745295-cf845d52-ca0b-4f7f-98ab-529a4533470f.jpg)
![cubecity4](https://user-images.githubusercontent.com/5803874/153745299-f10ac4f5-e3ea-4cf4-adc4-ee6ab65dbe4b.jpg)
![cubecity1](https://user-images.githubusercontent.com/5803874/153745285-07bcadcf-bfab-40cf-97dd-c91db66cc058.jpg)

## Overview:
Procedurally generate Cube City with manhattan style skyscrapers, roads, cars, stoplights, and helicopters of "ANY SIZE"!

## Implementation
![Cube City diagram](https://user-images.githubusercontent.com/5803874/156256669-fc3db5f4-8708-4918-bf10-ecfbf9ab4b22.jpg)

- Most of the implementation details are outline out in the Diagram above
- Most of the main elements call for and use the Model View Controller pattern (with the Models not using UnityEngine)
- Some command pattern, observer pattern, and recursive code is used
- Just one object with one script: CubeCity.cs needed in the scene
- The Developer can dynamically set city attributes such as the size of city blocks in X, Z, average number of Helicopters per block, etc
- Helicopter paths are discarded if they fly through buildings
- Uses and adapts from Bezier Path Creator free Asset Store Package for the paths
- Cars stop at stoplights with waiting coroutines getting delay info from stoplight, no OnUpdate functions 
- Switch between 3 different camera views to see the city, follow helicopters, or follow cars
