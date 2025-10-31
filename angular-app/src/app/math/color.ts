export class Color {
  constructor(public r: number, public g: number, public b: number, public a: number) {}

  static fromUint32(argb: number): Color {
    const r = argb & 0xff;
    const g = (argb >>> 8) & 0xff;
    const b = (argb >>> 16) & 0xff;
    const a = (argb >>> 24) & 0xff;
    return new Color(r, g, b, a);
  }

  toUint32(): number {
    const c = this.clamped();
    return c.r | (c.g << 8) | (c.b << 16) | (c.a << 24);
  }

  multiplyScalar(s: number): Color {
    return new Color(this.r * s, this.g * s, this.b * s, this.a * s);
  }

  add(color: Color): Color {
    return new Color(this.r + color.r, this.g + color.g, this.b + color.b, this.a + color.a);
  }

  clamped(): Color {
    return new Color(Color.clamp(this.r), Color.clamp(this.g), Color.clamp(this.b), Color.clamp(this.a));
  }

  private static clamp(value: number): number {
    return Math.max(0, Math.min(255, Math.round(value)));
  }
}
