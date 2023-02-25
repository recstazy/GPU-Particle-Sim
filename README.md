# Particle simulation on GPU using compute shaders in Unity
Used [this](https://developer.nvidia.com/gpugems/gpugems3/part-v-physics-simulation/chapter-29-real-time-rigid-body-simulation-gpus) article 
but simplified it to 2D circles, skipped torque calculations and used compute shaders instead of vertex + fragment + depth test

No optimizations done, stats in editor on my laptop RTX 3060:
* 250k particles - 250+ FPS
* 1M particles - 60+ FPS

### Has unstable behavior on bounds collision

https://user-images.githubusercontent.com/30838103/221378790-03eda2c6-c71c-4ac6-98e8-ac4bf7747146.mp4
