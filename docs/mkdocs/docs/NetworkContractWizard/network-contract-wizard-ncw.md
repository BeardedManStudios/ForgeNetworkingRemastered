# Network Contract Wizard (NCW)

The Network Contract Wizard is the new way for you to implement networking for your game/app.

To do so you will need to go to **Window->Forge Networking->Network Contract Wizard** as shown in the example below.

![open-ncw](images/ncw-1.jpg "Open NCW")

After opening the window you will be presented with this.

![ncw-main-view](images/ncw-2.jpg "NCW Main View")

1. 1)Toggle lighting on/off for the editor window only. This will make it easier for your eyes depending on what lighting situation you have.
2. 2)Create, this will make networked objects for you to use for your game/app. (You will spend most of your time hitting this beautifully designedbutton).
3. 3)Deletion, this will properly delete the networked objects that you have made.

**Note: You are able to tap any of the NetwrokObjects to modify them as well.**

**Creation Menu**

![create-network-object-view](images/ncw-3.jpg "Create Network Object View")

1. 1)Name field for you to name your networked object (Do not use the same name as ones already made)
2. 2)Add fields (This is where you would add variables to sync across the network)
3. 3)Add RPC (This is where you would create callbacks for other clients to messages/data being sent)

**Adding a Field**

![adding-a-field](images/ncw-4.jpg "Adding a Field")

1. 1)The name of the variable
2. 2)The type of variable this will be
3. 3)There are many different types to select from (This is just an example of selecting int for this variable) Note: The trash bin next to this variable will delete it.

**Adding a RPC**

![adding-a-rpc](images/ncw-5.jpg "Adding a RPC")

1. 1)Name of the RPC
2. 2)Arguments for this RPC that will be sent across the network.
3. 3)Value type that can be passed across the network.
4. 4)Deletion of this RPC
5. 5)Deletion of this Value Type
6. 6)Add more Value Types for the arguments

**Main Menu Deletion**

![deleting-a-network-object](images/ncw-6.jpg "Deleting a Network Object")

1. 1)This will trash this Networked Object correctly.
2. 2)You will be prompted with this window when doing so.

**Project Directory**

![generated-code-output-location](images/ncw-7.jpg "Generated Code Output Location")

You will notice that all generated code will be located in your project directory under **Generated**.

Note: This will be changeable from the editor in the future.