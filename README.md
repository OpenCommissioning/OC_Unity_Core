# Open Commissioning Unity Package Guide

Welcome to the repository of the Open Commissioning Unity Package. It is part of the [Open Commissioning](https://github.com/OpenCommissioning) Project.
This document provides detailed instructions on how to install,
set up, and utilize the various components and features of the package within Unity.

**Table Of Content**

- [Open Commissioning Unity Package Guide](#open-commissioning-unity-package-guide)
    * [Installation](#installation)
        + [Dependencies](#dependencies)
        + [Installation Methods](#installation-methods)
        + [Requirements](#requirements)
    * [Package Contents](#package-contents)
    * [Setting Up a New Project](#setting-up-a-new-project)
    * [Demo Scene](#demo-scene)
    * [Modeling Machine Components](#modeling-machine-components)
        + [Modular Architecture](#modular-architecture)
        + [Modeling Motion](#modeling-motion)
            - [Axis](#axis)
            - [Actors](#actors)
                * [Cylinder](#cylinder)
                * [Drive Position](#drive-position)
                * [Drive Speed](#drive-speed)
                * [Drive Simple](#drive-simple)
        + [Modeling Sensors](#modeling-sensors)
            - [Sensor Binary](#sensor-binary)
                * [Static Collider](#static-collider)
            - [Sensor Analog](#sensor-analog)
            - [Measurement Components](#measurement-components)
                * [Measurement Angle](#measurement-angle)
                * [MeasurementEncoder](#measurementencoder)
        + [Interactions](#interactions)
            - [Button](#button)
            - [Switch Rotary](#switch-rotary)
                * [Switch Rotary Animation](#switch-rotary-animation)
            - [Lamp](#lamp)
            - [Color Changer](#color-changer)
            - [Lock](#lock)
            - [Door](#door)
    * [Modeling Material Flow](#modeling-material-flow)
        + [Payload](#payload)
        + [Handling Payloads](#handling-payloads)
            - [Gripper](#gripper)
            - [Payload Storage](#payload-storage)
        + [Creating and Deleting Payloads](#creating-and-deleting-payloads)
        + [Pool](#pool)
            - [Source](#source)
            - [Sink](#sink)
        + [Transporting Payloads](#transporting-payloads)
            - [Transport Linear](#transport-linear)
            - [Transport Curved](#transport-curved)
            - [Guided Payload](#guided-payload)
    * [Handling Data of Payloads](#handling-data-of-payloads)
        + [Payload Data](#payload-data)
        + [Payload Data Reader](#payload-data-reader)
        + [Payload Tag](#payload-tag)
            - [Product Data Directory Manager](#product-data-directory-manager)
        + [Tag Reader](#tag-reader)
    * [Connection to _TwinCAT_](#connection-to-twincat)
        + [Client](#client)
    * [Defining the Device Hierarchy](#defining-the-device-hierarchy)
        + [Example Project Hierarchy:](#example-project-hierarchy)
- [Contributing](#contributing)
    * [Submitting Pull Requests](#submitting-pull-requests)
    * [Code Style Convention](#code-style-convention)
    * [Guidelines for Contributions](#guidelines-for-contributions)


## Installation

### Dependencies

The package has the following dependencies which need to be added to the project's manifest.json:

```json
"com.dbrizov.naughtyattributes": "https://github.com/dbrizov/NaughtyAttributes.git#upm",
"com.cysharp.unitask": "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask"
```

### Installation Methods

There are three ways to install the package to a Unity project:

1. Install via git URL by adding this entry in the project's manifest.json:
   ```json
   "com.open-commissioning.core": "https://github.com/OpenCommissioning/OC_Unity_Core.git#upm"
   ```

2. Clone the repository, check out the upm branch and add the package using the Unity package manager.

3. Download the tar.gz archive of a release (Link to Release page) and add the package using the Unity package manager.

### Requirements

- Basic knowledge of using the Unity Editor, working with GameObjects, Prefabs, and Scripts and Events.
- Unity 2022.3 or later versions.

## Package Contents

The Unity Package contains:

- Scripts: Custom scripts for modeling specific elements and devices of the machine.
- Prefabs: Pre-configured GameObjects that encapsulate specific machine components.
- Assets: Resources such as 3D models, textures, and materials for creating realistic representations of machine components.

## Setting Up a New Project

1. Install the Package: Import the Unity Package into your Unity project.
2. Apply Default Layers: Click on Open Commissioning → Apply Default Layers in the Toolbar to import predefined layers and set the required collision matrix.

## Demo Scene

Open Commissioning  provides a Demo Scene in its Samples containing the most used components for modeling.
This can act as a reference how to structure and configure a project.

It features:
- Device components: [`Cylinders`](#cylinder), [`Drives`](#actors), and Sensors ([`Sensor Binary`](#sensor-binary), [`Sensor Analog`](#sensor-analog))
- Interaction components: [`Buttons`](#button), [`Lamp`](#lamp)
- Material flow elements: [`Source`](#source), [`Sink`](#sink), [`Payload Data Reader`](#payload-data-reader),
  [`Gripper`](#gripper), [`Transport Linear`](#transport-linear), [`Transport Curved`](#transport-curved)
- Components for creating twincat projects: [`Hierarchy`](#defining-the-device-hierarchy) for defining the structure, and [`Client`](#client) for configuration file generation

![DemoScene.gif](Documentation%2FImages%2FDemoScene.gif)
![DriveSpeed_Inspector.png](Documentation%2FImages%2FDriveSpeed_Inspector.png)
## Modeling Machine Components

### Modular Architecture

In Open Commissioning, a machine is represented as a hierarchical structure, with the smallest building block being a device. Each sensor and actor in the machine is represented by an individual device. These devices can be organized into modules, submodules or other hierarchical structures, allowing for a modular construction of complex systems.

### Modeling Motion

#### Axis

The fundamental building block for modeling motion in the simulation is the `Axis` component. This component receives a Target value and moves its GameObject depending on that value and its settings.
The target value can be provided by a referenced Actor component but can also be set from other components.

This principle pursues a separation between actors or other components providing a target value and their kinematic effects
in the simulation. This enables flexible connections of Actors and Axes with each other, which makes it possible to
model complex mechanisms and kinematic chains.

![Axis_Inspector.png](Documentation%2FImages%2FAxis_Inspector.png)

**Properties**:
- Override: When active, the Target value can be set manually in the Editor
- Target: Provided by the referenced actor. Units are meters [m] for Position controlled Translation axes or
  Degrees [°] for position controlled Rotation axes and [m/s] and [°/s] for speed controlled axes, respectively
- Value: The actual value of the `Axis`.
- Actor: The actor that provides a target value to the `Axis` component.
- Factor: This value gets multiplied with the target value, resulting in the actual value of the `Axis`.
- Direction: The `Axis` of the local coordinate system of the GameObject will be moved.
- Type: Translation or Rotation.
- Control Mode: Position or Speed.

#### Actors

Actor components can provide a target value to one or more `Axis` components.

##### Cylinder
This device component is an actor that provides a value
between a minimum and maximum value and is controlled by two signals, extend or retract.
The Cylinder can be referenced in an [`Axis`](#axis) component as its _Actor_.

![Cylinder_Inspector.png](Documentation%2FImages%2FCylinder_Inspector.png)

**Properties**:
- Control: Enables manual control of the component when the "Override" Button is toggled
- Progress: Visualizes the current position of the `Cylinder` in relation to the minimum and maximum values
- Value: The current value of the `Cylinder` which gets sent to referencing axes
- Limits: Sets the minimal _(X)_ and maximal _(Y)_ values of the `Cylinder`. Unit: _[m]_
- Type: Specify the type of `Cylinder` (double-acting, single-acting positive, single-acting negative)
- Time to Min/Max: Time in seconds to reach the minimal/maximal value when extended/retracted
- Profile: The movement profile of the `Cylinder` (default is linear)
- Communication: Properties for configuring the communication with its behavior model running in _TwinCAT_

**Events**:
- _On Active Changes_: Invoked when the active state changes.
- _On Limit Min/Max_: Invoked when the minimal/maximal value is reached.

![Cylinder_In_Action.gif](Documentation%2FImages%2FCylinder_In_Action.gif)
A `Cylinder` component sending a Target value to a linear [`Axis`](#axis) component.


##### Drive Position
The Drive Position component is a device that provides a target value modeling a movement to
a controlled position with a specified speed.
When an [`Axis`](#Axis) component is referencing this component in its _Actor_ property, it will perform this movement in the simulation.

![DrivePosition_Inspector.png](Documentation%2FImages%2FDrivePosition_Inspector.png)

**Properties**:
- Control: Enables manual control of the component when the "Override" Button is toggled
- Status: Displays if the Actor is active and the current value that is sent to the referencing [`Axes`](#axis)
- Speed: The speed of the drive. Units are _[m/s]_ or _[°/s]_ for translation or rotation axes, respectively
- Communication: Properties for configuring the communication with its behavior model running in _TwinCAT_

**Events**:
- _On Active Changed_: Invoked when the drive starts or stops moving.

##### Drive Speed
The Drive Speed component is a device that provides a target value representing
a movement to a controlled speed with a specified acceleration.
When an [`Axis`](#Axis) component is referencing this component in its _Actor_ property, it will perform this movement in the simulation.

![DriveSpeed_Inspector.png](Documentation%2FImages%2FDriveSpeed_Inspector.png)

**Properties**:
- Control: Enables manual control of the component when the "Override" Button is toggled.
- Status:
    - Is Active: Indicates if the Actor is active
    - Value: The actual value of the Drive
- Acceleration: The acceleration with which the actual value rises until the target value is reached.
- Communication: Properties for configuring the communication with its behavior model running in _TwinCAT_.

**Events**:
- _On Active Changed_: Invoked when the drive starts or stops moving.

The following gif shows how to manually control the `Drive Speed` component. This is part of the [Demo Scene](#demo-scene).

![DriveSpeed_Action.gif](Documentation%2FImages%2FDriveSpeed_Action.gif)

##### Drive Simple
The Drive Simple component is a device that provides a target value rising to a specified Speed with a specified Acceleration with two controls, forward and backward.
When an [`Axis`](#Axis) component is referencing this component in its _Actor_ property, it will perform this movement in the simulation.

![DriveSimple_Inspector.png](Documentation%2FImages%2FDriveSimple_Inspector.png)

**Properties**:
- Control:
    - Backward: The value raises to the specified speed with the specified acceleration in negative direction
    - Forward: The value raises to the specified speed with the specified acceleration in positive direction
- Status:
    - Is Active: Indicates if the Actor is active
    - Value: The actual value of the Drive
- Speed: The maximal speed value that the drive will reach.
- Acceleration: The acceleration with which the actual value rises until the maximal value is reached.
- Communication: Properties for configuring the communication with its behavior model running in _TwinCAT_.

**Events**:
- On Active Changed: Invoked when the drive starts or stops moving.

### Modeling Sensors

#### Sensor Binary
This device component can detect a collision between its detection zone and another collider of a
[`Payload`](#payload) component and sends a corresponding value to its behavior model.
The detection zone is modeled as a Unity Box Collider component
which gets automatically added and its size configured when the `Sensor Binary` component is added.

![SensorBinary_Inspector.png](Documentation%2FImages%2FSensorBinary_Inspector.png)

**Properties**:
- Control: Enables manual control of the component when the "Override" Button is toggled.
- Status:
    - Collision: Displays if a collision is detected
    - Value: Displays the Value that is sent to its behavior model
- Settings:
    - Group ID: Set an ID number to define which Payloads are detected by the sensor.
    - Collision Filter: Define which categories of Payloads are detected.
    - Length: Defines the length of the detection Zone.
    - Invert: Defines whether an inverted signal gets sent to its behavior model
    - Use Box Collider: If checked, the box collider component can be configured with custom settings and is used for detecting a collision
- Communication: Properties for configuring the communication with its behavior model running in _TwinCAT_.

**Events**:
- On Value Changed Event: Invoked when the Value Property changed
- On Collision Event: Invoked when a Collision is detected
- On Payload Enter Event: Invoked when a Payload component is entering the detection zone
- On Payload Exit Event: Invoked when a Payload component is leaving the detection zone


**_Note_**: To visualize the detection zone of the `Sensor Binary` component, the `Sensor Beam` component
can be added to the same GameObject.
This renders a line that changes color depending on the collision value of the `Sensor Binary` Component.

![SensorBeam_Inspector.png](Documentation%2FImages%2FSensorBeam_Inspector.png)

##### Static Collider

This component is used to model elements that need to trigger `Sensor Binary` components but are not [`Payload`](#payload)
themselves, for example mechanical triggers for binary sensors in a mechanism.

![StaticCollider_Inspector.png](Documentation%2FImages%2FStaticCollider_Inspector.png)

**Properties**:
- Group Id: Specifies the group ID of the Static Collider. This ID is relevant for the detection of the Static Collider component by a `Sensor Binary`component.


#### Sensor Analog
This device component models sensors reading analog values and sending those to their behavior model.
Within this component, a [`Measurement`](#measurement-components) component can be referenced as the value source.
This supplies the `Sensor Analog` component with the value that gets send to its behavior model.

The `Sensor Analog` components also provides a public function to set its value that can be called from other components.

![SensorAnalog_Inspector.png](Documentation%2FImages%2FSensorAnalog_Inspector.png)

**Properties**:
- Control: Enables manual control of the component when the "Override" Button is toggled.
- Status:
    - Value: The value that gets sent to the behavior model of the Sensor Analog component
- Settings:
    - Value Source: Measurement component from which the value is read from
    - Factor: A factor that gets multiplied with the measurement device value resulting in the Sensor Analog component's value
- Communication: Properties for configuring the communication with its behavior model running in _TwinCAT_.

**Events**:
- On Value Changed Event: Invoked when the Value of the Sensor Analog is changed.

#### Measurement Components

There are several Measurement components that can be referenced in the Sensor Analog Component:

##### Measurement Angle

Provides a value corresponding with the angle in a specified direction of a specified GameObject.

![MeasurementAngle_Inspector.png](Documentation%2FImages%2FMeasurementAngle_Inspector.png)

**Properties**:
- Settings:
    - Target: The GameObject of which the angle value gets read from
    - Offset: Offset value that gets added to the angle value
    - Direction: Specifies the direction of the angle which gets read
- State:
    - Value: The value that gets sent to the referencing Sensor Analog component

##### MeasurementEncoder

Provides a value corresponding to the value of a specified [`Drive`](#drive-position) component.

![MeasurementEncoder_Inspector.png](Documentation%2FImages%2FMeasurementEncoder_Inspector.png)

**Properties**:
- State:
    - Value: The value that gets sent to the referencing Sensor Analog component
- Settings:
    - Drive: The Drive component of which the value gets read from
    - Drive Type: Specifies the Type of the Drive, Position or Speed
    - Factor: Factor which the Drive value gets multiplied with
    - Modulo: Modulo Value with which the value sent to the Sensor Analog component gets calculated


Another important Measurement component is the Data
Reader component which can read data from a Payload component.
This component is described in its own [Section](#payload-data).

### Interactions

Interaction components model devices that can be interacted with by the operator like buttons, lamps or doors.

#### Button
This component is a device that models a Button that can be pressed or toggled and sends a
corresponding signal to its behavior model in twincat.
The interaction takes place by clicking in the colored circle with the left mouse button.

![Button_Inspector.png](Documentation%2FImages%2FButton_Inspector.png)

This component is a device that models a Button that can be pressed or toggled and sends a corresponding signal to its behavior model in twincat.
The interaction takes place by clicking in the colored circle with the left mouse button.

**Properties**:
- Status:
    - Pressed: Indicates if the `Button` is pressed
    - Feedback: Indicates the feedback value from the behavior model which indicates that the value was received
- Settings:
    - Local Feedback: If checked, the Feedback property will be set to the same value as Pressed
    - Type:
        - Click: The Button will return to its neutral position after 0.1 seconds after the mouse button is released
        - Toggle: The Button will stay engaged after the mouse button is released, requiring another click to return to its neutral position
- Visual:
    - Default: The default visualization
    - Safety: Visualization for an emergency stop button
    - Color: Specifies the color of the `Button`
    - Color Changers: Specifies Color Changer components that change the color of a rendered simulation object to model a visual feedback in the 3D visualization when the button is pressed
- Communication: Configures the configuration of the communication with the behavior model in _TwinCAT_

**Events**:
- On Click Event: Invoked when the `Button` is pressed
- On Pressed Changed: Invoked when the Pressed property changes
- On Feedback Changed: Invoked when the Feedback property changes

#### Switch Rotary

This device component is used model a rotary switch with multiple switch positions.

![SwitchRotary_Inspector.png](Documentation%2FImages%2FSwitchRotary_Inspector.png)

**Properties**:
- Status:
    - Index: The current switch position
    - Angle: The current angle of the switch
- Settings:
    - State Count: The number of possible states of the switch
    - Range: The range of rotation of the switch in degrees. The switch positions are divided equally over this range.
    - Offset: An initial offset of the switch angle
- Communication: Configures the configuration of the communication with the behavior model in _TwinCAT_

**Events**:
- On Rotation Changed: Invoked when the rotation changes
- On Index Changed: Invoked when the switch position changes

##### Switch Rotary Animation

This component can reference a Switch Rotary component and rotate depending
on the angle of it to visualize the rotation in the 3D visualisation.

![SwitchRotaryAnimation_Inspector.png](Documentation%2FImages%2FSwitchRotaryAnimation_Inspector.png)

**Properties**:
- State:
    - Angle: The current angle
- Settings:
    - Switch: The [`Switch Rotary`](#switch-rotary) component of which the rotation is visualized
    - Duration: The duration in seconds to move between different switch positions
    - Direction: The direction of the rotation

#### Lamp

This device component can be used to model a signal lamp.

![Lamp_Inspector.png](Documentation%2FImages%2FLamp_Inspector.png)

**Properties**:
- Control: When the "Override" Button is active, the signal of the lamp can be set manually by pressing the "Signal" Button
- Status:
    - Value: Indicates the current signal value of the lamp
- Settings:
    - Color: The color of the `Lamp` which will also set the color of all referenced [`Color Changer`](#color-changer) components
    - Color Changers: List of [`Color Changer`](#color-changer) components that will be controlled by the `Lamp` component.
- Communication: Configures the configuration of the communication with the behavior model in _TwinCAT_

Events:
- On Value Changed: Invoked when the value of the `Lamp` changes

#### Color Changer

This component changes the color of all rendered child objects to.
This component is can be controlled by other components via changing its public properties using events.

![ColorChanger_Inspector.png](Documentation%2FImages%2FColorChanger_Inspector.png)

**Properties**:
- Control:
    - Enable: Changes the color to a color with the specified Emission.
- Settings:
    - Color: The base color
    - Emission: The emission value

#### Lock

This device component is used to model a lock to control doors and up to four integrated buttons.

![Lock_Inspector.png](Documentation%2FImages%2FLock_Inspector.png)

**Properties**:
- Control: When the "Override" Button is active, the signal of the lock signal can be set manually by pressing the "Lock" Button
- Status:
    - Lock: Indicates if the `Lock` is locked.
    - Closed: Indicates if all referenced [`Doors`](#door) are closed
    - Locked: Indicates if all referenced [`Doors`](#door) are locked. This means that all doors are closed and the Lock signal is true.
- References:
    - Doors: All [`Door`](#door) components that the `Lock` is controlling
    - Buttons: All [`Button`](#button) components that are assigned to the `Lock` component
- Communication: Configures the configuration of the communication with the behavior model in _TwinCAT_

**Events**:
- On Lock Changed: Invoked when the _Lock_ property changes
- On Closed Changed: Invoked when the _Closed_ property changes
- On Locked Changed: Invoked when the _Locked_ property changes

#### Door

This component is used to model a door that can be opened and closed.

![Door_Inspector.png](Documentation%2FImages%2FDoor_Inspector.png)

**Properties**:
- Control:
    - Open: Opens the `Door` if it is not locked
    - Close: Closes the `Door`
- Status:
    - Closed: Indicates if the `Door` is closed
    - Lock: Indicates if the `Door` received a Lock signal
    - Locked: Indicates if the `Door` is closed and the Lock signal is true.
- Settings:
    - Target: The target position to which the `Door` is moved when it is opened. When it is closed it will move back to its initial position.
    - Duration: Duration in seconds of the movement to the Target position and back

**Events**:
- On Open Event: Invoked when the `Door` is opened
- On Close Event: Invoked when the `Door` is closed

## Modeling Material Flow

The basic element for modeling material flow are [`Payload`](#payload) components.
Those components model elements in the simulation that represent payloads like parts or assemblies or
other elements related to payloads like storage slots.
When the [`Payload`](#payload) component is added to a GameObject, a unity Rigidbody component and a BoxCollider component are
added automatically to that GameObject.

There are several components to model Payloads and Material Flow

### Payload

This is the basic component for modeling payloads inside the machine or process.

![Payload_Inspector.png](Documentation%2FImages%2FPayload_Inspector.png)

**Properties**:
- Settings:
    - Category:
        - Part: Represents a single part
        - Assembly: Represents an assembly which can include multiple parts
        - Transport: Represents a transport element on which a part or assembly can be transported on
    - Control State: Indicates the current state of the `Payload`. This can be changed by other components to model steps in a process.
    - Physics State: Specifies how the physics engine is handling the simulation of the attached rigidbody and box collider component.
        - Free: Rigidbody is not kinematic and is affected by gravity. The Collider is not a trigger
        - Parent: The Rigidbody is kinematic. The Collider is a trigger
        - Static: The Rigidbody is kinematic. The Collider is not a trigger
    - Data: The Payload component can carry different IDs to give a more granular control to handling it
        - Type Id: Specifies the Type ID of the Payload.
        - Unique Id: Specifies a unique ID of the Payload. This can be used to differentiate between multiple `Payload` components
        - Group Id: Specifies the group ID of the Payload. This ID is relevant for the detection of the `Payload` component by a [`Sensor Binary`](#sensor-binary) component.


### Handling Payloads

#### Gripper
Using this component [`Payload`](#payload)
components can be picked up, moved, and placed. When this component is added to a GameObject, a unity Rigidbody component and Box Collider component are added automatically to that GameObject. The Gripper component can detect Payload components which it can pick up. To pick up Payload components their colliders have to collide with the grippers collider.

When a Gripper is picking up [`Payload`](#payload)
components, those become child objects of the gripper in the unity hierarchy.

![Gripper_Inspector.png](Documentation%2FImages%2FGripper_Inspector.png)

**Properties**:

* Control: Pick up [`Payload`](#payload)
  components or place them
* Status:
    * Collision: indicates a collision between the grippers collider and the [`Payload`](#payload)
      components collider
    * Is Active: indicates if the `Grippers` Pick function is active
    * Is Picked: indicates if a [`Payload`](#payload)
      component is picked
* Collision:
    * Pick Type: Defines which [`Payload`](#payload)
      Type the Gripper component can detect and pick.
    * Dynamic Size: Change the Size of the Collider when the Grippers Pick function is active
    * Additional Collider Size: The dimensions of the additional collider if Dynamic Size is active
* Settings:
    * Group ID: Set an ID to define which [`Payloads`](#payload) are detected by the Gripper component. Only [`Payloads`](#payload) with the same Group ID are detected.
    * Collision Filter: Specifies which Type of [`Payload`](#payload)
      components are detected by the `Gripper`

**Events**:
* On Collision Event: gets invoked when the gripper detects a collision with a [`Payload`](#payload)
  component
* On Payload Enter Event: gets invoked when a [`Payload`](#payload)
  component is entering the `Grippers` collider
* On Payload Exit Event: gets invoked when a [`Payload`](#payload)
  component is exiting the `Grippers` collider
* On Pick Event: gets invoked when the Pick function of the `Gripper` is called
* On Place Event: gets invoked when the Place function of the `Gripper` is called
* On Is Active Changed Event: gets invoked when the `Gripper` Is Active property changes
* On is Picked Event: gets invoked when the `Gripper` Is Picket property changes

When the `Gripper` is placing a [`Payload`](#payload) component, it gets placed as a
child object in the unity hierarchy depending on what component is detected by the `Gripper` at its position.

If another [`Payload`](#payload)
components with type Assembly or Transport or `Payload Storage` component is detected,
the [`Payload`](#payload)
components becomes child object of that and its physics state will be set to Parent.

If none of the above-mentioned components are detected, the [`Payload`](#payload)
component will be placed as a child
object of the [`Pool`](#pool) component and its physics state will be set to Free.

#### Payload Storage

This component represents a container element where Payload components can be placed at.

![PayloadStorage_Inspector.png](Documentation%2FImages%2FPayloadStorage_Inspector.png)

It can be configured with a Group Id, specifying by which Detector (for example a `Gripper` component) can detect it.
When a child object is added to it, the On Children Add event gets invoked.

### Creating and Deleting Payloads

To create and delete [`Payload`](#payload)
components during the simulation, two components are provides:
[`Source`](#source) and `Sink`. Both these components require a `Pool` component to be present in the scene.

### Pool

![Pool_Inspector.png](Documentation%2FImages%2FPool_Inspector.png)

This component manages and tracks all the [`Payload`](#payload)
components that are created by [`Source`](#source) components
during the simulation. It contains a List of [`Payload`](#payload)
components that can be created by the [`Source`](#source)
components. This list corresponds to all different [`Payloads`](#payload) that can be created by [`Source`](#source) components.


#### Source

The `Source` component can be used to create a new [`Payload`](#payload)
component when the simulation is running.
It provides two functions to create a [`Payload`](#payload)
component and to delete the [`Payload`](#payload)

component within it. The area of the `Source` component is defined by an automatically added BoxCollider.

When created by a `Source`  component, a [`Payload`](#payload) component's GameObject becomes a child of the [`Pool`](#pool)
component's GameObject.

![Source_Inspector.png](Documentation%2FImages%2FSource_Inspector.png)

**Properties**:
- Status
    - Collision: indicating a collision between the source and a [`Payload`](#payload)
      component.
      If a collision is detected it means that there already is a [`Payload`](#payload)
      object in the source area and no new one can be created.
- Settings
    - Type ID: Defines which [`Payload`](#payload)
      component is created by the [`Source`](#source). This ID corresponds with the index of the Payload list in the [`Pool`](#pool) component.
    - Unique ID: Set the unique ID of the [`Payload`](#payload)
      component that is created by the [`Source`](#source). Default is 0 meaning the unique ID of the next created Payload components will be increased by 1 from the last one.
    - Auto Mode: if activated, the source will create a new [`Payload`](#payload)
      component as soon as no collision is detected
    - Collision Filter: define which [`Payload`](#payload)
      component types can be detected by the Source
      **Events**:
    - On Collision Event: gets invoked when a collision in the [`Source`](#source) area is detected
    - On Payload Enter Event: gets invoked when a [`Payload`](#payload)
      component is entering the [`Source`](#source) area
    - On Payload Exit Event: gets invoked when a [`Payload`](#payload)
      component is exiting the [`Source`](#source) area
    - On Payload Created: gets invoked when a [`Payload`](#payload)
      component is created by the [`Source`](#source)


#### Sink

The `Sink` component is able to delete [`Payload`](#payload) components detected in its area.
The area is defined by a unity BoxCollider component.

![Sink_Inspector.png](Documentation%2FImages%2FSink_Inspector.png)

**Properties**:
- Control:
    - Delete: deletes the [`Payload`](#payload)
      components within its area
- Status:
  -Collision: indicates that a [`Payload`](#payload)
  component is detected within the `Sink` area
- Settings:
    - Auto Mode: if activated, the `Sink` will automatically delete every [`Payload`](#payload)
      component detected in its area
    - Group ID: define the Group ID of the [`Payload`](#payload)
      components to be detected
    - Collision Filter: define the Type of [`Payload`](#payload)
      components to be detected
      **Events**:
    - On Collision Event: gets invoked when a collision in the `Sink` area is detected
    - On Payload Enter Event: gets invoked when a [`Payload`](#payload)
      component is entering the `Sink` area
    - On Payload Exit Event: gets invoked when a [`Payload`](#payload)
      component is exiting the `Sink` area


### Transporting Payloads

To model the transport of [`Payload`](#payload)
components within the simulation,
two types of transport surface components are available.
They are able to move all [`Payload`](#payload)
components in contact with them along their surface.
Transport components need to reference an Actor component like a [`Drive`](#drive-simple) which provides the target value
for the Transport component. This value represents the speed with which the [`Payloads`](#payload) are moved.

When a Transport component is added to a GameObject, a Unity BoxCollider component is added automatically.
This collider models the dimension of the Transport surface on which the [`Payload`](#payload)
components are moved.

There are two variants of the Transport component: [`Transport Linear`](#transport-linear) and [`Transport Curved`](#transport-curved).

#### Transport Linear

![TransportLinear_Gizmos.png](Documentation%2FImages%2FTransportLinear_Gizmos.png)

![TransportLinear_Inspector.png](Documentation%2FImages%2FTransportLinear_Inspector.png)

**Properties**:
- Control:
    - Target: Target value of the `Transport Linear` component.
- Status:
    - Value: The current value of the `Transport Linear` component. This value represents the speed in [m/s].
- Settings:
    - Actor: The Actor component that is controlling the `Transport Linear` component.
    - Length, Width, Height: Specifies the dimensions of the box collider representing the `Transport Linear` surface.
    - Factor: This value gets multiplied by the target value, resulting in the actual value of the `Transport Linear` surface speed.
    - Is Guiding: When active, Guided [`Payload`](#payload)
      components will be guided on the transport surface.
    - Is Dynamic: This option should be checked when the `Transport Linear` surface itself can move inside the simulation (e.g., when modeling a lifting conveyor).
      The [`Payload`](#payload)
      components on the surface will become child objects of the `Transport Linear` object and will be moved with the surface.

#### Transport Curved

The `Transport Curved` component works similarly to the [`Transport Linear`](#transport-linear) component and can move [`Payload`](#payload)
components along a curved line.

![TransportCurved_Gizmos.png](Documentation%2FImages%2FTransportCurved_Gizmos.png)

![TransportCurved_Inspector.png](Documentation%2FImages%2FTransportCurved_Inspector.png)

**Properties**:
- Control:
    - Target: Target value of the `Transport Curved` component.
- Status:
    - Value: The current value of the `Transport Curved` component. This value represents the speed in [m/s].
- Settings:
    - Actor: The Actor component that is controlling the `Transport Curved` component.
    - Angle: Sets the angle of the curved surface. The range is from -90 to 90 degrees.
    - Radius: Sets the radius of the curved surface. The minimum value is half of the Width value.
    - Width: Sets the width of the transport surface.
    - Height: Sets the height of the transport surface.
    - Factor: This value gets multiplied by the target value, resulting in the actual value of the `Transport Curved` surface speed.
    - Is Guiding: When active, [`Guided Payload`](#guided-payload) components will be guided on the `Transport Curved` surface.
    - Is Dynamic: This option should be checked when the `Transport Curved` surface itself can move inside the simulation (e.g., when modeling a lifting conveyor).
      The [`Payload`](#payload)
      components on the surface will become child objects of the `Transport Curved` object and will be moved with the surface.

#### Guided Payload

Both [Transport](#transporting-payloads) components are able to guide `Guided Payload` components along their central axis if the _Is Guided_ property is active.
The `Guided Payload` sends out a Raycast to detect the transport surface with which it is in contact with. If this Transport component has the `Is Guided` property activated,
the `Guided Payload` will be guided along their central axis.

![GuidedPayload_Inspector.png](Documentation%2FImages%2FGuidedPayload_Inspector.png)

**Properties**:
- Collision
    - Transport: This indicates the current guided Transport component on which the [`Payload`](#payload)
      is placed on. This will get detected by the Raycast.
      Settings
- Raycast Length: Sets the length in [m] of the Raycast looking for a Transport component. This length should be longer than the distance from the center of the GameObject to the collider edge of the BoxCollider.
- Raycast Layer: Sets on which Layers the Raycast will detect Transport components.
- Show Gizmos: When active, a Line representation of the Raycast is drawn as a Gizmo.


## Handling Data of Payloads

### Payload Data

[`Payload`](#payload)
components can be supplemented with additional data to model specific
properties of the [`Payload`](#payload)
relevant to the simulation, such as temperature, weight or other inherent properties.
To do this, the `Payload Data` component needs to be added to the same GameObject.

![PayloadData_Inspector.png](Documentation%2FImages%2FPayloadData_Inspector.png)

The `Payload Data` component is configured with a list of key-value pairs representing the required properties
of the [`Payload`](#payload)
component. These values can be read and modified by other components,
reflecting changes in the properties during the simulation process.


### Payload Data Reader

Data from the [`Payload Data`](#payload-data) component can be accessed using the `Payload Data Reader` component.
This component detects Payloads within its detection zone and reads data from their Payload Data components.
The detection zone is defined by an automatically added BoxCollider.

![PayloadDataReader_Inspector.png](Documentation%2FImages%2FPayloadDataReader_Inspector.png)

**Properties**:
- Control:
    - Read: Attempts to read the value of the key-value pair from the [`Payload Data`](#payload-data) component within the detection zone.
    - Write: Attempts to write the value specified in Target Data to the key-value pair of the [`Payload Data`](#payload-data) component in the detection zone.
    - Target Data: Specifies the value to be read or written.
- Status:
    - Collision: Indicates a collision, meaning a [`Payload`](#payload)
      component is within the detection zone.
    - Raw Data: Displays the value that has been read from the [`Payload Data`](#payload-data) component.
    - Value: Displays the value if the raw data can be parsed to a float.
- Settings:
    - Key: Specifies the key of the key-value pair in the detected [`Payload Data`](#payload-data) component that the `Payload Data Reader` component reads from or writes to.
    - Auto Read: If checked, the component attempts to read data as soon as a [`Payload`](#payload)
      component is detected.
    - Cyclic: The component attempts to read data every update cycle.
    - Group ID: The group ID of [`Payload`](#payload)
      components that should be detected.
    - Collision Filter: Specifies which [`Payload`](#payload)
      component types are detected.

**Events**:
- On Value Changed Event: Invoked when the Value of the component changes.
- On Collision Event: Invoked when a collision is detected.
- On Payload Enter Event: Invoked when a [`Payload`](#payload)
  component enters the detection zone.
- On Payload Exit Event: Invoked when a [`Payload`](#payload)
  component leaves the detection zone.

The Data Reader component also functions as a measurement device that can be referenced in a
[`Sensor Analog`](#sensor-analog)
component to make the data value available in _TwinCAT_.

### Payload Tag

The `Payload Tag `component functions as a data tag, such as an RFID tag, attached to a [`Payload`](#payload).
When a [`Payload`](#payload)
with a `Payload Tag` is created using a [`Source`](#source) component, a corresponding tag data file is generated on disk,
named after the [`Payload`](#payload)
_unique ID_. This file initially contains default values derived from a template.
The locations for both the template file and the generated tag file are defined in the _Directory Id_ property of
the component, which corresponds to the index of the Product `Data Directory Manager`. When the path begins with the prefix `streamingassets:`, 
the path will be interpreted as relative to the [StreamingAssets](https://docs.unity3d.com/Manual/StreamingAssets.html) folder.

![PayloadTag_Inspector.png](Documentation%2FImages%2FPayloadTag_Inspector.png)

![ProductDataDirectoryManager_Inspector.png](Documentation%2FImages%2FProductDataDirectoryManager_Inspector.png)

The `Data Directory Manager` component contains a List of directories that can be set.

The template file used for creating tag files is an XML file with the following schema:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Type>
	<Entry Type="UINT32" Key="MDS_UniqueId">00000000</Entry>
	<Entry Type="CHARS" Key="Chars Data 1">RFID</Entry>
	<Entry Type="BYTES" Key="Bytes Data 1">0x22</Entry>
	<Entry Type="INT16" Key="Int Data 1">23</Entry>
	<Entry Type="INT32" Key="Int Data 2">234253</Entry>
	<Entry Type="UINT16" Key="Int Data 3">24</Entry>
	<Entry Type="UINT32" Key="Int Data 4">2452566</Entry>
</Type>
```

Each _<Entry>_ element has two attributes, _Type_ and _Key_, representing the type and identifier of the entry.
The values of the entries are the default values with which every tag file is initialized.
The exception is the entry with the key _MDS_UniqueId_, whose value will be the _Unique ID_ property of the [`Payload`](#payload)
component.
The tag file itself is a `.data` file, containing an array of bytes, which is not readable by a text editor.
To read the tag file of a [`Payload Tag`](#payload-tag) component the auxiliary function in the Menu _Open Commissioning > Product Data > Data Viewer_ can be used:

![ProductDataViewer.png](Documentation%2FImages%2FProductDataViewer.png)

#### Product Data Directory Manager

This component is required to generate tag files for [`Payload Tag`](#payload-tag) components. The tag files are stored in the directories specified in this component.

![ProductDataDirectoryManager_Inspector.png](Documentation%2FImages%2FProductDataDirectoryManager_Inspector.png)

### Tag Reader

The Tag Reader component is a device designed to read the unique ID  property of a detected [`Payload`](#payload) component
when a [`Payload Tag`](#payload-tag) component is attached to that same GameObject.

![TagReader_Inspector.png](Documentation%2FImages%2FTagReader_Inspector.png)

**Properties**:

- Control:
    - Override: If activated, the value of the `Tag Reader` can be manually set.
- Status:
    - Collision: Indicates a collision, meaning a [`Payload`](#payload) component is in the detection zone.
    - Unique ID: Displays the unique ID of the [`Payload`](#payload) component within its detection zone.
- Settings
    - Group ID: Specifies the group ID of [`Payload`](#payload) components that should be detected.
    - Collision Filter: Specifies which [`Payload`](#payload) component types are detected.
    - Hold Value: If checked, the last value that was read is retained even after the [`Payload`](#payload)
      component exits the detection zone.
      **Events**:
    - On Value Changed Event: Invoked when the value of the component changes.
    - On Collision Event: Invoked when a collision is detected.
    - On Payload Enter Event: Invoked when a [`Payload`](#payload) component enters the detection zone.
    - On Payload Exit Event: Invoked when a [`Payload`](#payload) component leaves the detection zone.

**_NOTE:_**

While both the [`Payload Tag`](#payload-tag) & [`Tag Reader`](#tag-reader) and `Payload Data` & `Payload Data Reader` components handle data
from [`Payload`](#payload)
components, they serve different purposes:

- [`Payload Tag`](#payload-tag) & [`Tag Reader`](#tag-reader): This combination models RFID tag data handling.
  The `Tag Reader` component detects the unique ID of a detected [`Payload`](#payload)
  component when a [`Payload Tag`](#payload-tag) component is attached to the same GameObject.
  The content of the RFID tag resides only in the tag file, not within the Unity simulation itself.
  Data manipulation occurs through the [`Tag Reader`](#tag-reader)'s behavior model in _TwinCAT_, minimizing the need for data exchange between Unity and _TwinCAT_.

- [`Payload Data`](#payload-data) & [`Payload Data Reader`](#payload-data-reader): These components handle internal or physical properties of a [`Payload`](#payload),
  such as temperature, dimensions, or optical characteristics.
  To make this data accessible to a _TwinCAT_ behavior model,
  the [`Payload Data Reader`](#payload-data-reader)` must be referenced in a [`Sensor Analog`](#sensor-analog)
  component as its _Value Source_,
  which maintains a connection to _TwinCAT_.

The key difference is that the [`Payload Tag`](#payload-tag) and [`Tag Reader`](#tag-reader) combination focuses on handling RFID tag data,
where the tag content is stored in a file and managed through the Tag Reader's _TwinCAT_ behavior model.
The [`Payload Data`](#payload-data) and [`Payload Data Reader`](#payload-data-reader) deal with the [`Payload`](#payload)'s internal properties,
which are made accessible to _TwinCAT_ through the [`Sensor Analog`](#sensor-analog) component.


## Connection to _TwinCAT_

To establish a connection between the Unity simulation and a _TwinCAT_ project running the behavior models of the
devices, a `Client` component is provided.
This component manages the connection and can also generate a _TwinCAT_ project containing the behavior models based
on the hierarchy defined in Unity.

### Client

![TcAdsClient_Inspector.png](Documentation%2FImages%2FTcAdsClient_Inspector.png)

**Properties**:

- Config:
    - Name: Sets the name.
    - Net Id: Sets the ADS Net Id.
    - Task Port: Port on which the Task is running.
- Functions:
    - Connect: Attempts to connect to _TwinCAT_.
    - Disconnect: Disconnects the `Client` from _TwinCAT_.
    - Create Configuration: Creates a configuration file containing the hierarchical structure of the machine's _devices_ with their names and corresponding Function Block for their behavior model.
    - Update _TwinCAT_ Project: Triggers the Assistant application to update its connected _TwinCAT_ project based on the hierarchical structure of the machine's devices.
      Internally, this uses the same configuration file that can be created using _Create Configuration_.

## Defining the Device Hierarchy

The _device_ hierarchy defines the hierarchical structure of all _devices_ in the project.
This structure is used to create the _TwinCAT_ project with the _device_ behavior models and is defined
in Unity by attaching the `Hierarchy` component to GameObjects.
When generating the structure, the [`Client`](#client) component traverses through all its child objects in the Unity
Project tree and collects all _device_ components, which will be included in the _device_ structure.
For every GameObject with the `Hierarchy` component attached to it, a group within the _device_ hierarchy is created.
All devices of the `Hierarchy` component's children are included within that group.

![Hierarchy_Inspector.png](Documentation%2FImages%2FHierarchy_Inspector.png)

**Properties**:

- Name: Sets the name of the corresponding group element in the configuration file. If no name is set, the name of the GameObject is used.
- Is Name Sampler: If checked, the names of the devices in the GameObject's children will include their parent's GameObject name,
  separated by an underscore ("_").
- Parent: If not specified, the corresponding group in the configuration file
  will be under the group associated with the `Hierarchy` component of its parent object in the Unity structure.
  If a `Hierarchy` is specified in this property, the corresponding group will be under the group of that specified `Hierarchy` component.

### Example Project Hierarchy:

![ExampleHierarchy.png](Documentation%2FImages%2FExampleHierarchy.png)

This simple GameObject structure contains four _device_ components on the following GameObjects:
- On _BG_SensorBinary_ : [`Sensor Binary`](#sensor-binary)
- On _MA_Drive_ : [`Drive Simple`](#drive-simple)
- On _MB_Cylinder_ : [`Cylinder`](#cylinder)
- On _BT_TemperatureSensor_ : [`Sensor Analog`](#sensor-analog)

The GameObject named "Machine" contains the Client component, while the GameObjects
_Module_1_, _SubModule_1_, and _Module_2_ do **not** contain a [`Hierarchy`](#defining-the-device-hierarchy)
component.

When the configuration file is created using the [`Client`](#client)'s _Create Configuration_ function,
it is a flat list of devices:

```xml
<Main>
  <Device Name="BG_SensorBinary" Type="FB_LinkSensorBinary" Comment="" />
  <Device Name="BT_TemperatureSensor" Type="FB_LinkSensorAnalog" Comment="" />
  <Device Name="MA_Drive" Type="FB_LinkDrive" Comment="" />
  <Device Name="MB_Cylinder" Type="FB_LinkCylinder" Comment="" />
</Main>
```


When the   [`Hierarchy`](#defining-the-device-hierarchy)
component is then added to the GameObjects
_Module_1_, _SubModule_1_, and _Module_2_, the configuration file is created with nested groups reflecting
the hierarchical structure:

```xml
<Main>
  <Group Name="Module_1">
    <Device Name="MA_Drive" Type="FB_LinkDrive" Comment="" />
    <Group Name="SubModule_1">
      <Device Name="BG_SensorBinary" Type="FB_LinkSensorBinary" Comment="" />
    </Group>
  </Group>
  <Group Name="Module_2">
    <Device Name="BT_TemperatureSensor" Type="FB_LinkSensorAnalog" Comment="" />
    <Device Name="MB_Cylinder" Type="FB_LinkCylinder" Comment="" />
  </Group>
</Main>
```

In this example, groups based on the   [`Hierarchy`](#defining-the-device-hierarchy)
components in the project tree are created in the configuration file that contain
the devices located within the child objects of these groups.


# Contributing

We welcome contributions from everyone and appreciate your effort to improve this project.
We have some basic rules and guidelines that make the contributing process easier for everyone involved.

## Submitting Pull Requests

1. For non-trivial changes, please open an issue first to discuss your proposed changes.
2. Fork the repo and create your feature branch.
3. Follow the code style conventions and guidelines throughout working on your contribution.
4. Create a pull request with a clear title and description.

After your pull request is reviewed and merged.

> [!NOTE]  
> All contributions will be licensed under the project's license

## Code Style Convention

Please follow these naming conventions in your code:

| Type           | Rule             |
|----------------|------------------|
| Private field  | _lowerCamelCase  |
| Public field   | UpperCamelCase   |
| Protected field | UpperCamelCase   |
| Internal field | UpperCamelCase   |
| Property       | UpperCamelCase   |
| Method         | UpperCamelCase   |
| Class          | UpperCamelCase   |
| Interface      | IUpperCamelCase  |
| Local variable | lowerCamelCase   |
| Parameter      | lowerCamelCase   |
| Constant       | UPPER_SNAKE_CASE |

## Guidelines for Contributions

- **Keep changes focused:** Submit one pull request per bug fix or feature. This makes it easier to review and merge your contributions.
- **Discuss major changes:** For large or complex changes, please open an issue to discuss with maintainers before starting work.
- **Commit message format**: Use the [semantic-release](https://semantic-release.gitbook.io/semantic-release#commit-message-format) commit message format.
- **Write clear code:** Prioritize readability and maintainability.
- **Be consistent:** Follow existing coding styles and patterns in the project.
- **Include tests:** It is recommended to add or update tests to cover your changes.
- **Update examples:** If you think its helpful, include your new feature in the samples of the package.
- **Document your work:** Update relevant documentation, including code comments and user guides.

We appreciate your contributions and look forward to collaborating with you to improve this project!