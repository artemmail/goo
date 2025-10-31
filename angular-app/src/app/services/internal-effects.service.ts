import { Injectable } from '@angular/core';
import { Vector2 } from '../math/vector2';
import { Color } from '../math/color';

export interface BaseEffectSettings {
  filter: boolean;
  centerX: number;
  centerY: number;
  radius: number;
  shiftX: number;
  shiftY: number;
}

export interface SpikesSettings extends BaseEffectSettings {
  spikes: number;
}

export interface SmearSettings extends BaseEffectSettings {}

@Injectable({ providedIn: 'root' })
export class InternalEffectsService {
  spikes(
    target: Uint8ClampedArray,
    source: Uint8ClampedArray,
    width: number,
    height: number,
    settings: SpikesSettings
  ): void {
    const pos = new Vector2(settings.centerX, settings.centerY);
    const shift = new Vector2(settings.shiftX, settings.shiftY);
    const R = settings.radius;
    const k = settings.spikes;

    const maxAmp = R * 0.5;
    const amp = shift.length() / R;
    const dirY = amp !== 0 ? shift.normalized() : new Vector2(0, 1);
    const dirX = dirY.perpendicular();
    const scaleY = amp > maxAmp ? amp / maxAmp : 1;

    this.iteratePixels(target, width, height, (x, y, setPixel) => {
      const dx = x - pos.x;
      const dy = y - pos.y;
      let angle = Math.atan2(dy, dx);
      let radius = Math.hypot(dx, dy);

      if (radius < R) {
        const S = 1.0001 - amp * Math.sin(k * angle);
        const inner = Math.sqrt(S * S + (4 * radius / R) * (1 - S));
        radius = (R * (inner - S)) / (2 * (1 - S));
      }

      const sampleX = radius * Math.cos(angle) + pos.x;
      const sampleY = radius * Math.sin(angle) + pos.y;
      const color = this.sample(settings.filter, source, width, height, sampleX, sampleY);
      setPixel(color);
    });
  }

  smear(
    target: Uint8ClampedArray,
    source: Uint8ClampedArray,
    width: number,
    height: number,
    settings: SmearSettings
  ): void {
    const pos = new Vector2(settings.centerX, settings.centerY);
    const shift = new Vector2(settings.shiftX, settings.shiftY);
    const R = settings.radius;

    const maxAmp = R * 0.5;
    const amp = shift.length();
    const dirY = amp !== 0 ? shift.normalized() : new Vector2(0, 1);
    const dirX = dirY.perpendicular();
    const scaleY = amp > maxAmp ? amp / maxAmp : 1;

    this.iteratePixels(target, width, height, (x, y, setPixel) => {
      const dx = x - pos.x;
      const dy = y - pos.y;
      let xx = dx * dirX.x + dy * dirX.y;
      let yy = dx * dirY.x + dy * dirY.y;

      if (Math.abs(xx) < R && Math.abs(yy) < scaleY * R) {
        let ae = (3 * xx) / R;
        ae = amp * Math.exp(-ae * ae);
        const signY = yy >= ae ? 1 : -1;
        yy = (yy - ae) / (1 - (signY * ae) / (scaleY * R));
        const v = dirX.multiplyScalar(xx).add(dirY.multiplyScalar(yy));
        const sampleX = pos.x + v.x;
        const sampleY = pos.y + v.y;
        const color = this.sample(settings.filter, source, width, height, sampleX, sampleY);
        setPixel(color);
        return;
      }

      const color = this.sample(settings.filter, source, width, height, x, y);
      setPixel(color);
    });
  }

  private iteratePixels(
    target: Uint8ClampedArray,
    width: number,
    height: number,
    callback: (x: number, y: number, setPixel: (color: Color) => void) => void
  ): void {
    for (let y = 0; y < height; y++) {
      for (let x = 0; x < width; x++) {
        const offset = (y * width + x) * 4;
        callback(x, y, color => {
          const safe = color.clamped();
          target[offset] = safe.r;
          target[offset + 1] = safe.g;
          target[offset + 2] = safe.b;
          target[offset + 3] = safe.a;
        });
      }
    }
  }

  private sample(
    filter: boolean,
    source: Uint8ClampedArray,
    width: number,
    height: number,
    x: number,
    y: number
  ): Color {
    if (!filter) {
      const ix = Math.max(0, Math.min(width - 1, Math.round(x)));
      const iy = Math.max(0, Math.min(height - 1, Math.round(y)));
      return this.read(source, width, ix, iy);
    }

    const floorX = Math.floor(x);
    const floorY = Math.floor(y);
    const tx = x - floorX;
    const ty = y - floorY;

    const x0 = this.clampCoord(floorX, width);
    const x1 = this.clampCoord(floorX + 1, width);
    const y0 = this.clampCoord(floorY, height);
    const y1 = this.clampCoord(floorY + 1, height);

    const c00 = this.read(source, width, x0, y0);
    const c10 = this.read(source, width, x1, y0);
    const c01 = this.read(source, width, x0, y1);
    const c11 = this.read(source, width, x1, y1);

    const top = c00.multiplyScalar(1 - tx).add(c10.multiplyScalar(tx));
    const bottom = c01.multiplyScalar(1 - tx).add(c11.multiplyScalar(tx));
    const result = top.multiplyScalar(1 - ty).add(bottom.multiplyScalar(ty));
    return result;
  }

  private read(data: Uint8ClampedArray, width: number, x: number, y: number): Color {
    const offset = (y * width + x) * 4;
    return new Color(data[offset], data[offset + 1], data[offset + 2], data[offset + 3]);
  }

  private clampCoord(value: number, max: number): number {
    if (value < 0) {
      return 0;
    }

    if (value >= max) {
      return max - 1;
    }

    return value;
  }
}
