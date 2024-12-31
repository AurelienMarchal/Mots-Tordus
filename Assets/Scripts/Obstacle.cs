


public class Obstacle{


    public int x { get; private set; }
    public int y { get; private set; }

    public int width { get; private set; }

    public int height { get; private set; }

    public Obstacle(int x, int y, int width, int height) {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }
}