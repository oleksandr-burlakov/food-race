import { Injectable, signal, type OnDestroy } from '@angular/core';
import { UnleashClient } from 'unleash-proxy-client';
import { ENVIRONMENT } from '../../../environment/environment';

@Injectable({
  providedIn: 'root',
})
export class UnleashService implements OnDestroy {
  private unleash: UnleashClient;

  public isReady = signal<boolean>(false);
  public flags = signal<Record<string, boolean>>({});

  constructor() {
    this.unleash = new UnleashClient({
      url: ENVIRONMENT.unleash.url,
      clientKey: ENVIRONMENT.unleash.clientKey,
      appName: 'food-race-fe',
      refreshInterval: 15,
    });

    this.unleash.on('ready', () => {
      this.isReady.set(true);
      this.updateFlags();
    });

    this.unleash.on('update', () => {
      this.updateFlags();
    });

    this.unleash.start();
  }

  public isEnabled(flagName: string): boolean {
    return this.unleash.isEnabled(flagName);
  }

  public getVariant(flagName: string) {
    return this.unleash.getVariant(flagName);
  }

  public updateContext(userId?: string, customContext: Record<string, string> = {}) {
    this.unleash.updateContext({
      userId,
      ...customContext,
    });
  }

  private updateFlags() {
    const currentFlags: Record<string, boolean> = {};
    this.flags.set(currentFlags);
  }

  ngOnDestroy(): void {
    this.unleash.stop();
  }
}
