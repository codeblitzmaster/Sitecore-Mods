////(function (Speak) {
////    Speak.component({
////        name: "FormFieldsMapper",
////        initialize: function (app) {
////            debugger;
////        },
////        getTheListOfComponents: function (app) {
////            var componentsList = '';
////            $(Sitecore.Speak.app.components).each(function () { componentsList += '<li>' + this.id + '</li>'; });
////            $('.sc-CustomComponent[data-sc-id="' + this.id + '"]').html('<ul>' + componentsList + '</ul>');
////        }
////    });

////})(Sitecore.Speak);

(function (Speak) {
    require.config({
        paths: {
            mentionsInput: "/sitecore/shell/client/Applications/FormsBuilder/Layouts/Renderings/SitecoreMods/FormFieldsMapper/js/vendor/jquery.mentionsInput",
            mentionsInputCss: "/sitecore/shell/client/Applications/FormsBuilder/Layouts/Renderings/SitecoreMods/FormFieldsMapper/css/vendor/jquery.mentionsInput"
        }
    });

    var parentApp = window.parent.Sitecore.Speak.app.findApplication('EditActionSubAppRenderer');

    Speak.component(["itemJS", "bclCollection", "mentionsInput", "css!mentionsInputCss"],
        function (itemManager, Collection) {
            var getItem = function (guid, callBack, options) {
                if (!guid) {
                    callBack(null);
                    return;
                }

                options = options || {};
                options.language = Speak.Context.current().Language;
                options.database = Speak.Context.current().contentDatabase;

                itemManager.fetch(guid,
                    options,
                    function (data, result) {
                        if (result.statusCode === 401) {
                            Speak.module("bclSession").unauthorized();
                            return;
                        }
                        callBack(data);
                    });
            };
            var getChildren = function (guid, callBack, options) {
                if (!guid) {
                    callBack(null);
                    return;
                }

                options = options || {};
                options.language = Speak.Context.current().Language;
                options.database = Speak.Context.current().contentDatabase;

                itemManager.fetchChildren(guid, options, function (data, result) {
                    if (result.statusCode === 401) {
                        Speak.module("bclSession").unauthorized();
                        return;
                    }
                    console.log(data);
                    callBack(data);
                });
            };

            var getListItems =
                function (listItems, valueFieldName, displayFieldName, selectedValue, notFoundText) {
                    var currentField = {};
                    currentField[valueFieldName] = selectedValue;
                    if (selectedValue && !_.findWhere(listItems, currentField)) {
                        currentField[displayFieldName] = selectedValue + " - " + notFoundText;
                        currentField.notFound = true;
                        var items = listItems.slice(0);
                        items.unshift(currentField);

                        return items;
                    }

                    return listItems;
                };

            return Speak.extend({}, Collection.prototype, {
                //initialize: function () {
                //    debugger;
                //    this.defineProperty("FormFields", []);
                //    this.defineProperty("DestinationFields", []);

                //},
                initialized: function () {
                    Collection.prototype.initialized.call(this);
                    debugger;
                    this.defineProperty("FormFields", []);
                    this.defineProperty("DestinationFields", []);
                },
                setFormFields: function (formFields) {
                    var formFields = _.map(formFields, function (field) {
                        return {
                            id: field.itemId,
                            name: field.name
                        };
                    });
                    this.FormFields = formFields;
                    this.renderFormFields();
                },
                getFormFields: function () {
                    return this.FormFields;
                },
                setDestinationFields: function (item) {
                    options = {};
                    options.database = Speak.Context.current().contentDatabase;
                    var app = this;
                    getChildren(item.$itemId, function (items) {
                        this.DestinationFields = (items.length > 0) ? items : [];
                        var df = _.map(items, function (i) {
                            var destinationField = {
                                "Name": i.$itemName,
                                "DisplayName": i.$displayName,
                                "Description": i.Description,
                                "Type": i.Type,
                                "IsRequired": i.IsRequired == "1" ? true : false,
                                "IsPrimary": i.IsPrimary == "1" ? true : false,
                                "Value": "",
                                "ID": i.$itemId
                            };
                            app.Items.push(destinationField);
                            return destinationField;
                        });
                        this.buildTable();
                        this.app.ProgressIndicatorPanel.IsBusy = false;
                    }.bind(this));

                    debugger;
                },
                build: function (fieldsRootItem, formFields) {
                    this.setFormFields(formFields);
                    this.setDestinationFields(fieldsRootItem);
                    
                    debugger;

                },
                buildTable: function () {
                    if (this.Items.length < 1) {
                        if ($('.form-fields-mapper-body .form-fields-mapper-msg').length < 1) {
                            $('.form-fields-mapper-body').append('<p class="form-fields-mapper-msg">No Fields available for mapping</p>');
                        }
                    }
                    else {
                        /*
                            <tr>
                                <td>
                                    <label for="firstname" class="required">FirstName</label>
                                    <span>Firstname of the person</span>
                                </td>
                                <td>
                                    <textarea id="firstname" required></textarea>
                                </td>
                            </tr>
                         */
                        var $tableBody = $("#form-fields-mapper tbody");
                        _.each(this.Items, function (item, idx) {
                            var $tr = $('<tr>');

                            var $destinationColumn = $('<td>');
                            var $destinationColumnLabel = $('<label>', {
                                for: item.Name,
                                "class": item.IsRequired ? "required" : null,
                                text: item.Name
                            });
                            $destinationColumn.append($destinationColumnLabel);
                            if (item.Description != "") {
                                var $destinationColumnDescription = $('<span>', {
                                    "class": "description",
                                    text: item.Description
                                });
                                $destinationColumn.append($destinationColumnDescription);
                            }
                            $tr.append($destinationColumn);

                            var $sourceColumn = $('<td>');
                            var $sourceColumnTextarea = $('<textarea>', {
                                id: item.Name,
                                text: item.Value,
                                "class": "form-control source-field",
                                required: item.IsRequired,
                                "data-row": idx
                            });
                            $sourceColumn.append($sourceColumnTextarea);
                            $tr.append($sourceColumn);
                            debugger;
                            $tableBody.append($tr);
                        });

                        var formFields = this.getFormFields();
                        console.log(formFields);

                        $tableBody.find(".source-field").mentionsInput({
                            allowRepeat: true,
                            onDataRequest: function (mode, query, callback) {
                                var data = formFields;
                                data = _.filter(data, function (item) { return item.name.toLowerCase().indexOf(query.toLowerCase()) > -1 });
                                callback.call(this, data);
                            },
                            templates: {
                                mentionItemSyntax: _.template('@[<%= value %>]'),
                            }
                        });
                        var app = this;
                        $tableBody.find(".source-field").on("input", function () {
                            var idx = $(this).data("row");
                            $(this).mentionsInput("val", function (newValue) {
                                //console.log(newValue);
                                app.Items[idx].Value = newValue;
                            });
                            //console.log("Updated Value:", app.Items[idx].Value);
                            app.validateItems();
                        });
                    }
                },
                renderFormFields: function () {
                    var formFieldsList = '';
                    $(this.FormFields).each(function () { formFieldsList += '<li>' + this.name + ' (' + this.id + ')' + '</li>'; });
                    $('.sc-FormFieldsMapper[data-sc-id="' + this.id + '"] .debug-info').html('<ul>' + formFieldsList + '</ul>');
                },
                getMappings: function () {
                    return this.Items;
                },
                validateItems: function () {
                    var mappings = this.getMappings();
                    var isValid = false;
                    isValid = _.every(mappings,
                        function (fieldMapping) {
                            if (fieldMapping.IsRequired) {
                                return fieldMapping.Value.length > 0;
                            }
                            return true;
                        });
                    parentApp.setSelectability(this, isValid);
                }
            });
        }, "FormFieldsMapper");
})(Sitecore.Speak);