import type { ApiRegistryEntry, ApiRegistryModule } from '@/generated/apiRegistry';
import { generatedApiRegistry } from '@/generated/apiRegistry';

export function registryDef(module: ApiRegistryModule, path: string): ApiRegistryEntry {
  const d = generatedApiRegistry.find((e) => e.module === module && e.path === path);
  if (!d) {
    throw new Error(`API registry missing entry ${module}/${path}. Run npm run generate:api-registry.`);
  }
  return d;
}
