#version 330 core
layout (location = 0) in vec2 aPosition;
uniform mat4 uProjection; // ћатрица дл€ перевода пикселей в координаты OpenGL

void main() {
    gl_Position = uProjection * vec4(aPosition, 0.0, 1.0);
}