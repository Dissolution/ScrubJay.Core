namespace ScrubJay.Scratch.Playground;

public class CircleA
{
    public static CircleB CircleB => new CircleB();
    public static CircleC CircleC => new CircleC();

    public static void CircleAMethod(){}
}

public class CircleB
{
    public static CircleA CircleA => new CircleA();
    public static CircleC CircleC => new CircleC();

    public static void CircleBMethod(){}
}

public class CircleC
{
    public static CircleA CircleA => new CircleA();
    public static CircleB CircleB => new CircleB();

    public static void CircleCMethod(){}
}
