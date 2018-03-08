# SmartCar

**SmartCar** is a test project for self driving cars. Cars learn how to drive using an **Unsupervised Neural Network (UNN)** & a **Generic Algorithm**.

### Prerequisites

* Install Unity3D (Only for Dev)

### Download

* [Download Link](https://github.com/Maugun/smartCar/releases)

## Built With

* [Unity3D](https://unity3d.com/) - Game Development Platform

## How to Use

![alt Config Screen](https://raw.githubusercontent.com/Maugun/smartCar/master/Samples/Config_Screen.PNG)

* **Population Size** - Number of Cars produced by Generation - **Default: 50**
* **Keep X best Cars of each Generation** - Number of Cars kept from one Generation to another. The other Cars will be mutated version of those kept Cars. - **Default: 20**

* **Layers** - Number of Neurons by Layer of the UNN. The first Layer is the **Input Layer** and is **always set to 6** (= number of car sensors). The last Layer is the **Output Layer** and is **always set to 2** (= v (acceleration/brake) & h (steering)). - **Default: [6, 10, 10, 2]**
* **Bias** - If set to True (checked) add a Bias to the UNN. - **Default: True**

## Authors

* **Richard ALLEMAN** - [Maugun](https://github.com/Maugun)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
