import { createModuleClient, envBase } from './createModuleClient';

const identity = envBase.identity;

export const identityClient = createModuleClient(identity, identity);
export const hmsClient = createModuleClient(envBase.hms, identity);
export const lisClient = createModuleClient(envBase.lis, identity);
export const lmsClient = createModuleClient(envBase.lms, identity);
export const pharmacyClient = createModuleClient(envBase.pharmacy, identity);
export const sharedClient = createModuleClient(envBase.shared, identity);
export const communicationClient = createModuleClient(envBase.communication, identity);
