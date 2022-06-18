# City-Viewer-Online
WebGL, C#, SQL, PHP, Database, Unity

View City Viewer Online here: [City Viewer Online](https://yalewinter.com/cityviewer/ "City Viewer Online") (WebGL Browser from desktop)

![city_viewer_1_2_blog](https://user-images.githubusercontent.com/5803874/173785430-926382bb-bb7a-44eb-b608-ba6f34af5251.jpg)

## Overview:
Procedurally generate a city with skyscrapers, roads, cars, stoplights, and helicopters. Using no art assets only scaled cube primitives.

**Features:**
- Cube primitives are procedurally instantiated, scaled and positioned as buildings for each block
- Roads are drawn between the blocks
- Stoplights are placed at the intersections
- Cars that drive around on the one-way roads, stop at red stoplights, and go on green
- Stoplights that change and animate the way they should with their different red yellow and green bulbs, and relay that info to the cars
- Helicopters that follows windy paths, circle big buildings, and don't crash into buildings
- Settings to modify the procedural generation of Cube City for different sizes and styles
- 5.14.22 Save to and load from cities saved online on a database on my website with SQL, PHP, HTTP requests.

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
