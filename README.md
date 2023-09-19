Commander Markers Static Hosting

Community shared marker presets

PR changes to the `Community/Markers.json` file.

File Schema
```
{
  "lastEdit":"string timestamp",
  "categories":{
        "category_name":"string",
        "markers":[
             {
                "author":"strinv",
                "name": "string",
                "description": "string", 
                "mapId": int,
                "trigger": {
                  "x": float,
                  "y": float,
                  "z": float
                },
                "markers": [
                  {
                    "i": int,
                    "d": "string",
                    "x": float",
                    "y": float,
                    "z": float
                  },
                ]
            }
        ]
    }
]
```
`category_name`: The grouping text for user filtering

Marker

`author`: Your display name

`name`: The marker set name

`desciption`: Tooltip/second line help information

`mapId`: Gw2Api id for the map on which the marker set exists

`trigger`: x,y,z information from Mumblelink where user needs to stand to activate the marker set from the map.
  * Map activation is limited to a 15 unit sphere around the trigger location. 
  * Map markers are also hidden when the user is more then 60 units away in the z direction. Used for "floor level" filters

`markers`: Array of up to 8 marker objects'

Marker Objects

`i`: Index of the commander marker to place
  - 0: Clear Markers,
  - 1: Arrow,
  - 2: Circle,
  - 3: Heart,
  - 4: Square,
  - 5: Star,
  - 6: Spiral,
  - 7: Triangle,
  - 8: Cross/X,
  - 9: Clear Markers

`d`: Marker Identifier
  - Used for remembering what the marker is for. Only shown in the library editor screen

`x` `y` `z`: Position information from Mumblelink



Notes:
Markers are placed by placing the mouse on the screen x/y coordinates that align with the Marker's position, and then virtually pressing the marker hotkey. Placement works on both the compass(minimap) and the full screen map.
Due to in-game level geometry, the game's hitscan for where to vertically place the markers can be wrong. Good examples are the Keineng Overlook stike mission. the first collible floor is well under the boss arena.









## Changelog



* 2023-09-19 Intial Community Markers Setup