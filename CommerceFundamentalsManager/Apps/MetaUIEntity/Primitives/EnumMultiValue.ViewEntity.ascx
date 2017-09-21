<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.BusinessFoundation.BaseEntityType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.EntityPrimitives.EnumMultiValue_ViewEntity" %>
<%@ Import Namespace="Mediachase.BusinessFoundation.Data.Meta" %>
<%@ Import Namespace="Mediachase.BusinessFoundation.Data.Meta.Management" %>
<%@ Import Namespace="Mediachase.BusinessFoundation.Core" %>
<%@ Import Namespace="Mediachase.BusinessFoundation.Data.Business" %>
<script language="c#" runat="server">
	protected string GetValue(EntityObject DataItem, string FieldName)
	{
		string retVal = "";

		if (DataItem != null && DataItem.Properties[FieldName] != null)
		{
			MetaField field = MetaDataWrapper.GetMetaFieldByName(DataItem.MetaClassName, FieldName);

			MetaFieldType type = field.GetMetaType();

			int[] idList = (int[])DataItem[FieldName];
			foreach (int id in idList)
			{
				if (retVal != String.Empty)
					retVal += "<br />";
				retVal += CHelper.GetResFileString(MetaEnum.GetFriendlyName(type, id));
			}
		}
		return retVal;
	}
</script>
<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-propertysheet">
	<tr>
	 	<td>
		  <%# GetValue(DataItem, FieldName) %>
		</td>
	</tr>
</table>