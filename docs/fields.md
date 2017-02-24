# Fields

The Network Object (most commonly seen as Network Object) is a variable found in the classes that were generated from using the Network Contract Wizard (NCW). When you create a field in the Network Contract Wizard (NCW) for a generated type, then it will be added as a property of that generated type. Below are the currently supported types of fields allowed in a network object field.

| **Field** | **Size** |
| --- | --- |
| byte | 8 bits |
| sbyte | 8 bits |
| short | 16 bits |
| ushort | 16 bits |
| int | 32 bits |
| uint | 32 bits |
| long | 64 bits |
| ulong | 64 bits |
| float | 32 bits |
| double | 64 bits |
| char | 8 bits |
| string | 32 bits + length * 8 |
| Vector | 96 bits |
| Vector2 | 64 bits |
| Vector3 | 96 bits |
| Vector4 | 128 bits |
| Quaternion | 128 bits |
| Color | 128 bits |

## Field Usage

When you generate a class it will be generated with a **networkObject** variable. This variable has all of the fields that you described in the Network Contract Wizard (NCW) built into it. For example, if you created a field in the Network Contract Wizard (NCW) that was named position and was selected to be a VECTOR3 then you will have access to it by doing **networkObject.position**. One thing that you will notice when doing this is that this field is actually a C# property, but we will explain this momentarily.

On some fields (such as VECTOR3) you will notice that it has a greyed out button labeled interpolate. If you were to click this, it will turn on interpolation for this field. You can set the interpolation time by assigning the text input field to the right of the button once it is active, default is 0.15. If you are not familiar with interpolation, basically when we send messages across the network we are dealing with millisecond gaps of information, this will cause objects to seem as though they were teleporting or lagging. By using interpolation, you can smooth out these movements to look more natural.

## Behind the Scenes

What is going on behind the scenes? Well let's start with where we left off in the last section on how the fields are actually properties. These properties have a getter/setter on each one. The getter will simply return a private field in the generated network object class; however, the setter does a few more actions. When you assign the value of the property, the setter will first set the private field to the value specified. Next a dirty flag will be set to tell the network to syndicate the change for that variable on the next network update for this object. What this dirty flag allows is for FNR to be able to only pick fields that have changed and send those across the network. This reduces the amount of data being sent by a lot depending on how often other variables are updated.