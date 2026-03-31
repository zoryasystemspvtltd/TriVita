import { Alert, Button, Stack, Typography } from '@mui/material';
import { useMutation } from '@tanstack/react-query';
import { Form, Formik } from 'formik';
import * as Yup from 'yup';
import { FormikTextField } from '@/components/common/FormikTextField';
import { PageHeader } from '@/components/layout/PageHeader';
import {
  assignRolePermissions,
  assignUserFacilities,
  assignUserRoles,
  createPermission,
  createRole,
  createUser,
} from '@/services/identityAdminService';
import { getApiErrorMessage } from '@/utils/errorMap';
import { useState } from 'react';

export function IdentityUsersView() {
  const [msg, setMsg] = useState<string | null>(null);
  const mUser = useMutation({ mutationFn: createUser });
  const mRoles = useMutation({ mutationFn: ({ id, roleIds }: { id: number; roleIds: number[] }) => assignUserRoles(id, roleIds) });
  const mFac = useMutation({
    mutationFn: ({ id, ids }: { id: number; ids: number[] }) => assignUserFacilities(id, ids),
  });

  return (
    <Stack spacing={3}>
      <PageHeader
        title="User management"
        subtitle="Identity admin APIs are action-oriented (create user, assign roles & facilities). No list endpoint in current API."
      />
      {msg ? <Alert severity="info">{msg}</Alert> : null}
      <Typography variant="subtitle1">Create user</Typography>
      <Formik
        initialValues={{
          tenantId: 1,
          email: '',
          password: '',
          facilityId: '',
          role: 'User',
        }}
        validationSchema={Yup.object({
          email: Yup.string().email().required(),
          password: Yup.string().min(8).required(),
        })}
        onSubmit={async (v, { resetForm }) => {
          setMsg(null);
          try {
            const res = await mUser.mutateAsync({
              tenantId: Number(v.tenantId),
              email: v.email,
              password: v.password,
              facilityId: v.facilityId ? Number(v.facilityId) : undefined,
              role: v.role,
              isActive: true,
            });
            setMsg(res.success ? `User id: ${res.data}` : res.message ?? 'Failed');
            resetForm();
          } catch (e) {
            setMsg(getApiErrorMessage(e));
          }
        }}
      >
        {({ isSubmitting }) => (
          <Form>
            <Stack direction="row" flexWrap="wrap" gap={2}>
              <FormikTextField name="email" label="Email" type="email" sx={{ minWidth: 220 }} />
              <FormikTextField name="password" label="Password" type="password" sx={{ minWidth: 200 }} />
              <FormikTextField name="tenantId" label="Tenant ID" type="number" sx={{ width: 120 }} />
              <FormikTextField name="facilityId" label="Facility ID (opt)" sx={{ width: 140 }} />
              <FormikTextField name="role" label="Legacy role label" sx={{ width: 160 }} />
              <Button type="submit" variant="contained" disabled={isSubmitting} sx={{ mt: 2 }}>
                Create
              </Button>
            </Stack>
          </Form>
        )}
      </Formik>
      <Typography variant="subtitle1">Assign roles (user id + comma-separated role ids)</Typography>
      <Formik
        initialValues={{ userId: 1, roleIds: '1' }}
        onSubmit={async (v, { resetForm }) => {
          setMsg(null);
          try {
            const ids = v.roleIds.split(/[\s,]+/).map(Number).filter(Boolean);
            const res = await mRoles.mutateAsync({ id: Number(v.userId), roleIds: ids });
            setMsg(res.success ? 'Roles assigned.' : res.message ?? 'Failed');
            resetForm();
          } catch (e) {
            setMsg(getApiErrorMessage(e));
          }
        }}
      >
        {({ isSubmitting }) => (
          <Form>
            <Stack direction="row" flexWrap="wrap" gap={2}>
              <FormikTextField name="userId" label="User ID" type="number" sx={{ width: 120 }} />
              <FormikTextField name="roleIds" label="Role IDs" sx={{ minWidth: 200 }} />
              <Button type="submit" variant="outlined" disabled={isSubmitting} sx={{ mt: 2 }}>
                Assign roles
              </Button>
            </Stack>
          </Form>
        )}
      </Formik>
      <Typography variant="subtitle1">Assign facilities (user id + comma-separated facility ids)</Typography>
      <Formik
        initialValues={{ userId: 1, facilityIds: '1' }}
        onSubmit={async (v, { resetForm }) => {
          setMsg(null);
          try {
            const ids = v.facilityIds.split(/[\s,]+/).map(Number).filter(Boolean);
            const res = await mFac.mutateAsync({ id: Number(v.userId), ids });
            setMsg(res.success ? 'Facilities assigned.' : res.message ?? 'Failed');
            resetForm();
          } catch (e) {
            setMsg(getApiErrorMessage(e));
          }
        }}
      >
        {({ isSubmitting }) => (
          <Form>
            <Stack direction="row" flexWrap="wrap" gap={2}>
              <FormikTextField name="userId" label="User ID" type="number" sx={{ width: 120 }} />
              <FormikTextField name="facilityIds" label="Facility IDs" sx={{ minWidth: 200 }} />
              <Button type="submit" variant="outlined" disabled={isSubmitting} sx={{ mt: 2 }}>
                Assign facilities
              </Button>
            </Stack>
          </Form>
        )}
      </Formik>
    </Stack>
  );
}

export function IdentityRolesView() {
  const [msg, setMsg] = useState<string | null>(null);
  const mRole = useMutation({ mutationFn: createRole });
  const mPerm = useMutation({
    mutationFn: ({ id, permissionIds }: { id: number; permissionIds: number[] }) =>
      assignRolePermissions(id, permissionIds),
  });
  const mCreatePerm = useMutation({ mutationFn: createPermission });

  return (
    <Stack spacing={3}>
      <PageHeader title="Role management" subtitle="Create roles, permissions, and assign permissions to roles." />
      {msg ? <Alert severity="info">{msg}</Alert> : null}
      <Typography variant="subtitle1">Create permission</Typography>
      <Formik
        initialValues={{
          tenantId: 1,
          permissionCode: '',
          permissionName: '',
          moduleCode: '',
        }}
        onSubmit={async (v, { resetForm }) => {
          setMsg(null);
          try {
            const res = await mCreatePerm.mutateAsync({
              tenantId: Number(v.tenantId),
              permissionCode: v.permissionCode,
              permissionName: v.permissionName,
              moduleCode: v.moduleCode || undefined,
            });
            setMsg(res.success ? `Permission id: ${res.data}` : res.message ?? 'Failed');
            resetForm();
          } catch (e) {
            setMsg(getApiErrorMessage(e));
          }
        }}
      >
        {({ isSubmitting }) => (
          <Form>
            <Stack direction="row" flexWrap="wrap" gap={2}>
              <FormikTextField name="permissionCode" label="Code (e.g. hms.api)" sx={{ minWidth: 200 }} />
              <FormikTextField name="permissionName" label="Display name" sx={{ minWidth: 200 }} />
              <FormikTextField name="moduleCode" label="Module (opt)" sx={{ width: 140 }} />
              <FormikTextField name="tenantId" label="Tenant ID" type="number" sx={{ width: 120 }} />
              <Button type="submit" variant="contained" disabled={isSubmitting} sx={{ mt: 2 }}>
                Create permission
              </Button>
            </Stack>
          </Form>
        )}
      </Formik>
      <Typography variant="subtitle1">Create role</Typography>
      <Formik
        initialValues={{ tenantId: 1, roleCode: '', roleName: '', description: '' }}
        onSubmit={async (v, { resetForm }) => {
          setMsg(null);
          try {
            const res = await mRole.mutateAsync({
              tenantId: Number(v.tenantId),
              roleCode: v.roleCode,
              roleName: v.roleName,
              description: v.description || undefined,
            });
            setMsg(res.success ? `Role id: ${res.data}` : res.message ?? 'Failed');
            resetForm();
          } catch (e) {
            setMsg(getApiErrorMessage(e));
          }
        }}
      >
        {({ isSubmitting }) => (
          <Form>
            <Stack direction="row" flexWrap="wrap" gap={2}>
              <FormikTextField name="roleCode" label="Role code" sx={{ width: 160 }} />
              <FormikTextField name="roleName" label="Role name" sx={{ minWidth: 200 }} />
              <FormikTextField name="description" label="Description" sx={{ minWidth: 200 }} />
              <FormikTextField name="tenantId" label="Tenant ID" type="number" sx={{ width: 120 }} />
              <Button type="submit" variant="outlined" disabled={isSubmitting} sx={{ mt: 2 }}>
                Create role
              </Button>
            </Stack>
          </Form>
        )}
      </Formik>
      <Typography variant="subtitle1">Assign permissions to role</Typography>
      <Formik
        initialValues={{ roleId: 1, permissionIds: '1' }}
        onSubmit={async (v, { resetForm }) => {
          setMsg(null);
          try {
            const ids = v.permissionIds.split(/[\s,]+/).map(Number).filter(Boolean);
            const res = await mPerm.mutateAsync({ id: Number(v.roleId), permissionIds: ids });
            setMsg(res.success ? 'Permissions assigned.' : res.message ?? 'Failed');
            resetForm();
          } catch (e) {
            setMsg(getApiErrorMessage(e));
          }
        }}
      >
        {({ isSubmitting }) => (
          <Form>
            <Stack direction="row" flexWrap="wrap" gap={2}>
              <FormikTextField name="roleId" label="Role ID" type="number" sx={{ width: 120 }} />
              <FormikTextField name="permissionIds" label="Permission IDs" sx={{ minWidth: 200 }} />
              <Button type="submit" variant="outlined" disabled={isSubmitting} sx={{ mt: 2 }}>
                Assign permissions
              </Button>
            </Stack>
          </Form>
        )}
      </Formik>
    </Stack>
  );
}
