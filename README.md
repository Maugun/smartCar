# SmartCar

**SmartCar** is a test project for self driving cars. Cars learn how to drive using an **Unsupervised Neural Network (UNN)** & a **Generic Algorithm**.

### Prerequisites

* Install Unity3D (Only for Dev)

### Download

* [Download Link](https://github.com/Maugun/smartCar/releases) (Recommended)
* [Web Version](https://maugun.github.io/smartCar/Web/index.html) (Not optimal)

## Built With

* [Unity3D](https://unity3d.com/) - Game Development Platform

## How to Use

### Setup Simulation

![alt Config Screen](https://raw.githubusercontent.com/Maugun/smartCar/master/Samples/Config_Screen.PNG)

* **1** - **Population Size** - Number of Cars produced by Generation - **Default: 50**
* **2** - **Keep X best Cars of each Generation** - Number of Cars kept from one Generation to another. The other Cars will be mutated version of those kept Cars. - **Default: 20**
* **3** - **Layers** - Number of Neurons by Layer of the UNN. The first Layer is the **Input Layer** and is **always set to 6** (= number of car sensors). The last Layer is the **Output Layer** and is **always set to 2** (= v (acceleration/brake) & h (steering)). - **Default: [6, 10, 10, 2]**
* **4** - **Bias** - If set to True (checked) add a Bias to the UNN. - **Default: True**
* **5** - Start the Simulation

### In Simulation

![alt Config Screen](https://raw.githubusercontent.com/Maugun/smartCar/master/Samples/Graph_Screen.PNG)

* **1** - Generation number.
* **2** - Best, average & worst **NN Fitness** of last generation. Fitness is the number of invisible checkpoint reached by the car in %.
* **3** - **Fitness by generation Graph**. - **Curves** - Bleu is best fitness, white is the average fitness, red is the worst fitness.
* **4** - Simulation Setup infos.
* **5** - Button to switch between Graph and Neural Network view.
* **6** - Button to pause generation.
* **7** - Press ESC to quit the game.

![alt Config Screen](https://raw.githubusercontent.com/Maugun/smartCar/master/Samples/Brain_Screen.PNG)

* **1** - **Best Neural Network of previous generation** display in real time. **Circle are neurons, Lines are weights**. Blue weights are negative, red weights are positive. A weight of 0 is fully transparent, a weight of 1 is fully opaque.
* **2** - Best Car of last generation (follow by an indicator) - Car of the same color are from the same species.
* **3** - **Inputs / Sensor** - F is forward sensor, B is backward sensor, R is right sensor, L is left sensor, TR is top right sensor, TL is top left sensor.
* **4** - **Outputs** - V is acceleration/brake, H is steering.
* **5** - **Bias** - Bias weights are set to 1

## Authors

* **Richard ALLEMAN** - [Maugun](https://github.com/Maugun)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
