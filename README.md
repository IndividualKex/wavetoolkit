# Wave Toolkit

This Unity package provides a simple, flexible implementation of the Wave Function Collapse algorithm.

## Installation

- In Unity, open the Package Manager (Window -> Package Manager)
- Click the plus (+) in the top left, then `Add package from git URL`
- Enter this repository url: `https://github.com/IndividualKex/wavetoolkit.git`

## Usage

- Create your modules
    - Create a module scriptable object in your assets folder (Right Click -> Create -> Wave Toolkit -> Module)
    - Drag and drop your actual module prefabs into the `Prefab` field
    - Define your connections, which currently are just based on string equivalency
- Create a gameobject and add a `Generator` component
- In the `Generator` inspector, click `Add Grid` to add a wave grid
- Add your modules to the `Modules` field
- Click `Generate`, or from script, call `Generator.instance.Generate()`, to generate your grids
