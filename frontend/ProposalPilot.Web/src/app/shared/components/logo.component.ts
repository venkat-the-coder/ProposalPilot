import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-logo',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="flex items-center gap-3" [class]="containerClass">
      <!-- Logo Image -->
      <img
        [src]="logoUrl"
        [alt]="'ProposalMind Logo'"
        [style.height.px]="iconSize"
        class="flex-shrink-0 object-contain w-auto"
      />

      <!-- Logo Text (Optional) -->
      @if (showText) {
        <div class="flex flex-col gap-1">
          <span
            [class]="textClass"
            class="font-extrabold tracking-tight leading-none"
          >
            <span class="bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent">
              Proposal
            </span>
            <span class="bg-gradient-to-r from-purple-600 to-pink-600 bg-clip-text text-transparent">
              Mind
            </span>
          </span>
          @if (showTagline) {
            <span class="text-xs text-gray-600 font-medium tracking-wide">
              AI-Powered Thinking
            </span>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    :host {
      display: inline-block;
    }
  `]
})
export class LogoComponent {
  @Input() iconSize: number = 48;
  @Input() showText: boolean = true;
  @Input() showTagline: boolean = false;
  @Input() textClass: string = 'text-3xl';
  @Input() containerClass: string = '';
  @Input() logoUrl: string = '/assets/logo.png';
}
