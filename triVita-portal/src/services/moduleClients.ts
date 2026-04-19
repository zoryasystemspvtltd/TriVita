import type { AxiosInstance } from 'axios';
import type { ApiRegistryModule } from '@/generated/apiRegistry';
import {
  communicationClient,
  hmsClient,
  identityClient,
  lisClient,
  lmsClient,
  pharmacyClient,
  sharedClient,
} from '@/services/http/clients';

export function getAxiosForModule(module: ApiRegistryModule): AxiosInstance {
  switch (module) {
    case 'hms':
      return hmsClient;
    case 'lis':
      return lisClient;
    case 'lms':
      return lmsClient;
    case 'pharmacy':
      return pharmacyClient;
    case 'shared':
      return sharedClient;
    case 'communication':
      return communicationClient;
    case 'identity':
      return identityClient;
    default: {
      const _: never = module;
      return _;
    }
  }
}
