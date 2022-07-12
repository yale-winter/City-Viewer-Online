# City-Viewer-Online
WebGL, C#, SQL, PHP, Database, HTTP, Unity

[View City Viewer Online Here (WebGL Browser for Mac or PC)](https://yalewinter.com/cityviewer/)

![city-viewer-online-screenshot](https://user-images.githubusercontent.com/5803874/178402932-6e00a948-c27d-4bc9-a0b0-0477fc079663.jpg)

## Overview:
Create a new City or load from Cities saved online. Cities are procedurally generated and have skyscrapers, roads, cars, stoplights, and helicopters. Using no art assets only scaled cube primitives.

**Features:**
- Cube primitives are procedurally instantiated, scaled and positioned as buildings for each block
- Roads are drawn between the blocks
- Stoplights are placed at the intersections
- Cars that drive around on the one-way roads, choose randomly to turn or go straight, stop at red stoplights, and go on green
- Stoplights that change and animate the way they should with their different red yellow and green bulbs, and relay that info to the cars
- Helicopters that fly around, don't crash into buildings
- Settings to modify the procedural generation of Cube City for different sizes and styles
- 5.14.22 Save to and load from cities saved online on a database on my website with SQL, PHP, HTTP requests.

## Implementation
![Cube City diagram](https://user-images.githubusercontent.com/5803874/156256669-fc3db5f4-8708-4918-bf10-ecfbf9ab4b22.jpg)

*The diagram above is now outdated but included as a reference point*

- Most of the implementation details are outline out in the Diagram above
- Some different design patterns are used where appropriate including Model View Controller
- Very few objects and scripts in the base scene (easily move canvas elements to a prefab to instantiated to reduce further, left in for ease of updates)
- The Player can dynamically set city attributes such as the size of city blocks in X, Z, average number of Helicopters per block, etc
- Helicopter possible paths are discarded if they fly through buildings
- Adapts from Bezier Path Creator free Asset Store Package for the Helicopter paths, and Boxophobic for the sky box image
- Cars stop at stoplights with waiting coroutines getting delay info from stoplight
- Switch between the different camera views to see the city, follow helicopters, or follow cars
