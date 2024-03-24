# Heritage3D

This project allows users to build digital museum experiences on top of an existing building, with various supporting tools and with the use of user uploaded media. Created museums can be navigated and expored freely, with various types of movement and item viewing. 

As mentioned, requires a "base" underlying building, which in this specific case was the Forte da Trafaria, but was ommited from the project files due to space constraints. The images found further down show the building present.

## Description

This Unity project was made as part of a thesis project, and the goal was to allow users to create and view online museums. As such, two types of users exist - Museum Builders and Museum Visitors. As the names imply, users can build museum experiences by adding their own media into the world, and enriching them with various tools, such as widgets. Furthermore, each museum can receive a guide path, allowing the builder users to guide visitors through the environment in a specific order.

Some of the added features include:
- Media Catalog - used to add, store and place user media
- Building slicing - allows fly mode to easily see inside the building
- Colliders & Snapping - allows builders to define physical boundries, as the base buildings had no colliders
- Portals - allows for the creation of a portal and teleportation grid
- 3D Item Widgets - enriches user placed items by making them less static
- Guide Path - allows builders to dictate a "viewing" path for the visitors

These features, and some others present in the project, can be seen in the video found in the next section.

## App preview

The [following video](https://www.youtube.com/watch?v=jm3DZe8ZERg) shows some of the available features present in the Builder mode of the application. Missing from the video are some features that are unique to the visitor mode, such as scene loading and Point & Click movement.


## Web integration

As mentioned, this Unity project was part of a larger project. This project is to be built as a WebGL application and integrated into a website, where users would be able to create an account, choose between build and visitor mode, and with the account, allow for uploaded media to be loaded on any computer, given that uploaded media is stored in a file database.
