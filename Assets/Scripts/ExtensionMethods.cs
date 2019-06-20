public static class ExtensionMethods {

    public static float Normalize(this float v, float a, float b, float min, float max) {
        return (b - a) * ((v - min) / (max - min)) + a;
    }
}