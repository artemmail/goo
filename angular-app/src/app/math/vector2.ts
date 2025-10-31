export class Vector2 {
  constructor(public x: number, public y: number) {}

  static zero(): Vector2 {
    return new Vector2(0, 0);
  }

  add(v: Vector2): Vector2 {
    return new Vector2(this.x + v.x, this.y + v.y);
  }

  subtract(v: Vector2): Vector2 {
    return new Vector2(this.x - v.x, this.y - v.y);
  }

  multiplyScalar(c: number): Vector2 {
    return new Vector2(this.x * c, this.y * c);
  }

  multiply(v: Vector2): Vector2 {
    return new Vector2(this.x * v.x, this.y * v.y);
  }

  divideScalar(c: number): Vector2 {
    return new Vector2(this.x / c, this.y / c);
  }

  dot(v: Vector2): number {
    return this.x * v.x + this.y * v.y;
  }

  length(): number {
    return Math.hypot(this.x, this.y);
  }

  normalized(): Vector2 {
    const len = this.length();
    if (len === 0) {
      return new Vector2(0, 0);
    }

    return this.divideScalar(len);
  }

  perpendicular(): Vector2 {
    return new Vector2(this.y, -this.x);
  }
}
