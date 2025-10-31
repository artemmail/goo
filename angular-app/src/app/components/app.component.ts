import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { InternalEffectsService, SpikesSettings, SmearSettings } from '../services/internal-effects.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  @ViewChild('canvas', { static: true }) canvasRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('preview', { static: true }) previewRef!: ElementRef<HTMLCanvasElement>;

  readonly spikes: SpikesSettings = {
    filter: true,
    centerX: 150,
    centerY: 150,
    radius: 100,
    spikes: 8,
    shiftX: 20,
    shiftY: 40
  };

  readonly smear: SmearSettings = {
    filter: true,
    centerX: 150,
    centerY: 150,
    radius: 80,
    shiftX: 60,
    shiftY: 0
  };

  activeEffect: 'spikes' | 'smear' = 'spikes';
  imageLoaded = false;

  constructor(private readonly effects: InternalEffectsService) {}

  ngOnInit(): void {
    const canvas = this.canvasRef.nativeElement;
    canvas.width = 300;
    canvas.height = 300;

    const ctx = canvas.getContext('2d');
    if (!ctx) {
      throw new Error('Unable to acquire 2D rendering context.');
    }

    const gradient = ctx.createLinearGradient(0, 0, 300, 300);
    gradient.addColorStop(0, '#ff0066');
    gradient.addColorStop(0.5, '#00ccff');
    gradient.addColorStop(1, '#ffee00');
    ctx.fillStyle = gradient;
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    this.imageLoaded = true;
    this.renderEffect();
  }

  renderEffect(): void {
    if (!this.imageLoaded) {
      return;
    }

    const sourceCtx = this.canvasRef.nativeElement.getContext('2d');
    const previewCtx = this.previewRef.nativeElement.getContext('2d');

    if (!sourceCtx || !previewCtx) {
      return;
    }

    const sourceData = sourceCtx.getImageData(0, 0, this.canvasRef.nativeElement.width, this.canvasRef.nativeElement.height);
    const targetData = previewCtx.createImageData(sourceData.width, sourceData.height);

    if (this.activeEffect === 'spikes') {
      this.effects.spikes(targetData.data, sourceData.data, sourceData.width, sourceData.height, this.spikes);
    } else {
      this.effects.smear(targetData.data, sourceData.data, sourceData.width, sourceData.height, this.smear);
    }

    previewCtx.putImageData(targetData, 0, 0);
  }
}
