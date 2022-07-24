using System;

public class Global {
    public static Random random;
    static Global() {
        random = new Random();
    }
    public static int GCD(int a, int b) {
        while (a != 0 && b != 0) {
            if (a > b) {
                a %= b;
            } else {
                b %= a;
            }
        }
        return a | b;
    }
}
